import { Title } from '@angular/platform-browser';
import { AUTORIZACIJA_DISABLED } from './../../constants';
import { ComponentCanDeactivate } from 'src/app/guards/pending-changes.guard';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatChip } from '@angular/material/chips';
import { Predmet } from 'src/app/models/predmet.model';
import { IspitniRok } from 'src/app/models/ispitni-rok.model';
import { ApiService } from '../../services/api.service';
import { ApiMsg } from "../../types/apiMsg.type";
import { tipoviBlanketa } from '../../constants';
import { PredmetService } from 'src/app/services/predmet.service';
import { AuthService } from 'src/app/services/auth.service';
import { BehaviorSubject, first, map, Observable, ReplaySubject, shareReplay, Subscription } from 'rxjs';
import { UserBasic } from 'src/app/models/user-basic.model';
import { Blanket } from 'src/app/models/blanket.model';
import { ActivatedRoute } from '@angular/router';
import { MatSlider } from '@angular/material/slider';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatDialog } from '@angular/material/dialog';
import { BlanketPreviewHandsetComponent } from 'src/app/components/blanket-preview-handset/blanket-preview-handset.component';

@Component({
  selector: 'blanketi',
  templateUrl: './blanketi.component.html',
  styleUrls: ['./blanketi.component.scss']
})
export class BlanketiComponent implements OnInit, OnDestroy {
  private subscription: Subscription;

  rokovi: IspitniRok[] | null | undefined;
  tipovi: string[] = tipoviBlanketa;

  selektovaniTipovi: string[] = [];
  selektovaniRokoviIds: number[] = [];
  godina: number = 2022;

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  isHandset: boolean = false;
  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => {
        this.isHandset = result.matches
        return this.isHandset
      }),
      shareReplay()
    );

  isExpanded: boolean = false;

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);
  user$: BehaviorSubject<UserBasic | null>;
  user: UserBasic | null;

  constructor(
    private dialog: MatDialog,
    private breakpointObserver: BreakpointObserver,
    public predmetService: PredmetService,
    private authService: AuthService,
    private title: Title) {

    this.user$ = this.authService.getUserObserver();

    this.user$.subscribe(u => this.user = u);

    this.predmetService.getPredmet().subscribe({
      next: p => {
        if (p) {
          this.title.setTitle(`${p?.naziv} - Blanketi • e-Čitaonica`)
          this.predmetService.getBlanketi();
          this.predmet$.next(p);
        }
      }
    })
  }

  ngOnDestroy(): void {
  }

  ngOnInit(): void {
    this.predmetService.getIspitniRokovi().subscribe({
      next: (res) => {
        this.rokovi = res;
      },
      error: (err: ApiMsg) => {
        console.log(err.msg);
      }
    });
  }

  profesorPredmeta(): boolean {

    return this.user!.tip === 'profesor'
      && this.predmet$.value?.profesori?.map(p => p.id).includes(this.user?.id) || false;
  }

  getGodine() {
    let godine: number[] = [];
    this.predmet$.value?.blanketi?.forEach(blanket => {
      let godina = this.parseGodina(blanket);
      if (!godine.includes(godina)) {
        godine.push(godina);
      }
    });

    return godine;
  }

  blanketOnClick(blanket: Blanket) {
    this.predmetService.getFullBlanket(blanket.id!).subscribe(b => {
      this.predmetService.izabraniBlanket$.next(b);
    });

    if (this.isHandset) {
      this.dialog.open(BlanketPreviewHandsetComponent, {
        data: this.predmetService.izabraniBlanket$,
        maxWidth: '100vw',
        maxHeight: '100vh',
        height: '100%',
        width: '100%',
        panelClass: 'full-screen-modal'
      })
    }
  }

  selektujTip(chip: MatChip) {
    const index = this.selektovaniTipovi.indexOf(chip.value);
    if (index >= 0) {
      this.selektovaniTipovi.splice(index, 1);
    } else {
      this.selektovaniTipovi.push(chip.value);
    }
    chip.toggleSelected();
    //this.filter();

    this.getGodine();
  }

  selektujRok(chip: MatChip, rok: number) {
    const index = this.selektovaniRokoviIds.indexOf(rok);
    if (index >= 0) {
      this.selektovaniRokoviIds.splice(index, 1);
    } else {
      this.selektovaniRokoviIds.push(rok);
    }

    chip.toggleSelected();
  }

  checkFilter(blanket: Blanket) {
    return (this.selektovaniRokoviIds.includes(blanket.ispitniRok.id)
      || this.selektovaniRokoviIds.length == 0) &&
      (this.selektovaniTipovi.includes(blanket.tip) || this.selektovaniTipovi.length == 0)
    //  && Number(blanket.datum.substring(0, 4)) === this.godina;
  }

  parseGodina(blanket: Blanket) {
    return Number(blanket.datum.substring(0, 4));
  }
}

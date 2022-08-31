import { MatSnackBar } from '@angular/material/snack-bar';
import { constants } from './../../constants';
import { DiskusijaService } from 'src/app/services/diskusija.service';
import { PredmetService } from 'src/app/services/predmet.service';
import { Component, Input, OnInit, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ToggleValueModel } from "src/app/types/ToggleValueModel.type";
import { Diskusija } from 'src/app/models/diskusija.model';
import { UserBasic } from 'src/app/models/user-basic.model';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'diskusija-preview',
  templateUrl: './diskusija-preview.component.html',
  styleUrls: ['./diskusija-preview.component.scss'],
})
export class DiskusijaPreviewComponent implements AfterViewInit {
  @Input('diskusija') diskusija: Diskusija;
  @Input('showPredmet') showPredmet = false;
  canCheck: boolean = false;

  @Output('zahvaljenaChange') zahvaljenaChange: EventEmitter<ToggleValueModel> =
    new EventEmitter();
  @Output('pracenaChange') pracenaChange: EventEmitter<ToggleValueModel> =
    new EventEmitter();
  user$: BehaviorSubject<UserBasic | null>;


  constructor(private authService: AuthService, public predmetService: PredmetService,
    private diskusijaService: DiskusijaService,
    private router: Router, private route: ActivatedRoute,
    private _snackbar: MatSnackBar) { }

  ngAfterViewInit(): void {
    setTimeout(_ => this.canCheck = true, 0);
  }

  ngOnInit(): void {
    this.user$ = this.authService.getUserObserver();
  }

  navigacijaKaDiskusiji() {
    if (this.router.url.includes('korisnik') || this.router.url.includes('pocetna')) {
      this.router.navigate([`/predmet/${this.diskusija.predmet?.id}/diskusije/${this.diskusija.id}`]);
    }
    else if (this.router.url.includes('resenja') || this.router.url.includes('zadatak')) {
      this.router.navigate([`../../${this.diskusija.id}`], { relativeTo: this.route });
    }
    else {
      this.router.navigate([`../diskusije/${this.diskusija.id}`], { relativeTo: this.route });
    }
  }

  onZahvaljenaClick() {
    this.diskusija.zahvaljena = !this.diskusija.zahvaljena;
    if (!this.diskusija.zahvaljena) this.diskusija.brZahvalnica--;
    else this.diskusija.brZahvalnica++;

    this.diskusijaService.zahvaliToggle(this.diskusija.id!).subscribe({
    })

    this.zahvaljenaChange.emit({
      korisnikId: this.user$.value?.id,
      newState: this.diskusija.zahvaljena,
      objectId: this.diskusija.id,
    });
  }

  onPracenaChange() {
    this.diskusija.pracena = !this.diskusija.pracena;

    const pracenjeText = this.diskusija.pracena ? "Dobićete obaveštenja o promeni diskusije." : "Nećete više dobijati obaveštenja o promeni diskusije.";

    this.diskusijaService.zapratiToggle(this.diskusija?.id!)
      .subscribe({
        next: msg => {
          this._snackbar.open(pracenjeText, 'OK', {
            ...constants.snackbarPosition,
            duration: 3000,
          });
        },

        error: _ => {
          this.diskusija.pracena = !this.diskusija.pracena;
        }
      });

    this.pracenaChange.emit({
      korisnikId: this.user$.value?.id,
      newState: this.diskusija.pracena,
      objectId: this.diskusija.id,
    });
  }

  onComment() {
    if (this.router.url.includes('korisnik') || this.router.url.includes('pocetna')) {
      this.router.navigate([`/predmet/${this.diskusija.predmet?.id}/diskusije/${this.diskusija.id}`], { state: { scrollAnchor: 'comment' } });
    }
    else if (this.router.url.includes('resenja') || this.router.url.includes('zadatak')) {
      this.router.navigate([`../../${this.diskusija.id}`], { state: { scrollAnchor: 'comment' }, relativeTo: this.route });
    }
    else {
      this.router.navigate([`../diskusije/${this.diskusija.id}`], { state: { scrollAnchor: 'comment' }, relativeTo: this.route });
    }
  }

  checkOverflow(quillRef: HTMLElement) {
    return quillRef.scrollHeight > quillRef.clientHeight
  }
}

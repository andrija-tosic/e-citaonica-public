import { Title } from '@angular/platform-browser';
import { AktivnostNiz } from './../../types/aktivnostNiz.type';
import { BreakpointObserver } from '@angular/cdk/layout';
import { DiskusijaService } from './../../services/diskusija.service';
import { OblastFilterService } from './../../services/oblast-filter.service';
import { PredmetService } from 'src/app/services/predmet.service';
import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject, map, Observable, ReplaySubject, shareReplay } from 'rxjs';
import { Diskusija } from 'src/app/models/diskusija.model';
import { Predmet } from 'src/app/models/predmet.model';
import { Router } from '@angular/router';
import { Student } from 'src/app/models/student.model';

@Component({
  selector: 'diskusije',
  templateUrl: './diskusije.component.html',
  styleUrls: ['./diskusije.component.scss'],
})
export class DiskusijeComponent implements OnInit {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);


  najaktivnijiStudenti: AktivnostNiz;

  constructor(private breakpointObserver: BreakpointObserver,
    private predmetService: PredmetService,
    private oblastFilterService: OblastFilterService,
    private router: Router,
    private title: Title) {
  }

  ngOnInit(): void {
    this.predmetService.getDiskusije();

    this.predmetService.getPredmet().subscribe({
      next: p => {
        if (p)
          this.title.setTitle(`${p?.naziv} - Diskusije • e-Čitaonica`)
        // p.diskusije!.unshift(diskusijaPrimer);
        // console.log(p);
        this.predmet$.next(p);

        if (p)
          this.predmetService.getNajaktivnijiStudenti(p.id).subscribe(studenti => {
            this.najaktivnijiStudenti = studenti;

            console.log(this.najaktivnijiStudenti);
          })
      }
    });
  }

  onFocus() {
    this.router.navigate(['./submit']);
  }

  checkFilter(diskusija: Diskusija) {
    const nijeIzabranaOblast: boolean = this.oblastFilterService.selektovaneOblasti$.value.length === 0;
    const nijeIzabranTip: boolean = this.oblastFilterService.selektovaniTipovi$.value.length === 0;

    if (!diskusija.zadatak) {
      const diskusijaSadrziOblasti = diskusija.oblasti?.some(oblast => this.oblastFilterService.selektovaneOblasti$.value.map(o => o.id).includes(oblast.id));

      return diskusijaSadrziOblasti || nijeIzabranaOblast;
    }
    else {
      const zadatakSadrziOblasti = diskusija.zadatak.oblasti.some(oblast => this.oblastFilterService.selektovaneOblasti$.value.map(o => o.id).includes(oblast.id));
      const zadatakJeTogTipa = this.oblastFilterService.selektovaniTipovi$.value.some(tip => diskusija.zadatak?.tip === tip);

      return (zadatakSadrziOblasti || nijeIzabranaOblast) && (zadatakJeTogTipa || nijeIzabranTip);
    }

  };
}
import { Title } from '@angular/platform-browser';
import { OblastFilterService } from './../../services/oblast-filter.service';
import { Observable, BehaviorSubject, ReplaySubject } from 'rxjs';
import { PredmetService } from './../../services/predmet.service';
import { Predmet } from 'src/app/models/predmet.model';
import { MatChip } from '@angular/material/chips';
import { Zadatak } from './../../models/zadatak.model';
import { Component, OnInit } from '@angular/core';
import { Oblast } from 'src/app/models/oblast.model';

@Component({
  selector: 'zadaci',
  templateUrl: './zadaci.component.html',
  styleUrls: ['./zadaci.component.scss']
})
export class ZadaciComponent implements OnInit {

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);

  constructor(private predmetService: PredmetService,
    private oblastFilterService: OblastFilterService,
    private title: Title) { }

  ngOnInit(): void {
    this.predmetService.getZadaci();
    this.predmetService.getPredmet().subscribe({
      next: p => {
        this.title.setTitle(`${p?.naziv} - Zadaci i pitanja • e-Čitaonica`)

        // console.log(p);
        this.predmet$.next(p);
      }
    });
  }

  checkFilter(zadatak: Zadatak) {
    const nijeIzabranaOblast: boolean = this.oblastFilterService.selektovaneOblasti$.value.length === 0;
    const nijeIzabranTip: boolean = this.oblastFilterService.selektovaniTipovi$.value.length === 0;

    const zadatakSadrziOblasti = this.oblastFilterService.selektovaneOblasti$.value.map(o => o.id)
      .some(oblastId => zadatak.oblasti.map(o => o.id).includes(oblastId));

    const zadatakSadrziTip = this.oblastFilterService.selektovaniTipovi$.value.some(tip => zadatak.tip === tip);

    return (zadatakSadrziOblasti || nijeIzabranaOblast) && (zadatakSadrziTip || nijeIzabranTip);
  };
}

import { OblastFilterService } from './../../services/oblast-filter.service';
import { Oblast } from './../../models/oblast.model';
import { MatChip, MatChipsDefaultOptions } from '@angular/material/chips';
import { ReplaySubject, first, BehaviorSubject } from 'rxjs';
import { Component, Input, OnInit } from '@angular/core';
import { Predmet } from 'src/app/models/predmet.model';

@Component({
  selector: 'oblast-filter',
  templateUrl: './oblast-filter.component.html',
  styleUrls: ['./oblast-filter.component.scss']
})
export class OblastFilterComponent implements OnInit {

  @Input('predmet') predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);

  selektovaneOblasti$: BehaviorSubject<Oblast[]> = new BehaviorSubject<Oblast[]>([]);
  selektovaniTipovi$: BehaviorSubject<string[]> = new BehaviorSubject<string[]>(['zadatak', 'pitanje']);

  constructor(private oblastFilterService: OblastFilterService) { }

  ngOnInit(): void {
    this.oblastFilterService.selektovaneOblasti$
      .subscribe(selektovaneOblasti => this.selektovaneOblasti$.next(selektovaneOblasti));

    this.oblastFilterService.selektovaniTipovi$
      .subscribe(tipovi => this.selektovaniTipovi$.next(tipovi));
  }

  selektujOblast(chip: MatChip, oblast: Oblast) {
    this.oblastFilterService.selektovaneOblasti$.subscribe(selektovaneOblasti => {
      const index = selektovaneOblasti.indexOf(oblast);

      if (index >= 0) {
        selektovaneOblasti.splice(index, 1);
      } else {
        selektovaneOblasti.push(oblast);
      }

      this.selektovaneOblasti$.next(selektovaneOblasti);
      // console.log(selektovaneOblasti);
      // this.filter();
      chip.toggleSelected();
    })
  }

  selektujTip(chip: MatChip, tip: string) {
    this.oblastFilterService.selektovaniTipovi$.subscribe(tipovi => {
      const index = tipovi.indexOf(tip);

      if (index >= 0) {
        tipovi.splice(index, 1);
      } else {
        tipovi.push(tip);
      }

      this.selektovaniTipovi$.next(tipovi);
    })
  }

  matChipSelected(oblast: Oblast) {
    return this.selektovaneOblasti$.value.map(o => o.id).includes(oblast.id);
  }

  matChipSelectedTip(tip: string) {
    return this.selektovaniTipovi$.value.includes(tip);
  }
}

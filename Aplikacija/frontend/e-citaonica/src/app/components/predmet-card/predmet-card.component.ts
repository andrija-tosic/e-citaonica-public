import { Component, Input, OnInit } from '@angular/core';
import { Predmet } from 'src/app/models/predmet.model';

@Component({
  selector: 'predmet-card',
  templateUrl: './predmet-card.component.html',
  styleUrls: ['./predmet-card.component.scss']
})
export class PredmetCardComponent implements OnInit {

  @Input() predmet: Predmet;
  @Input() isVertical: boolean = false;

  constructor() { }

  skraceniNazivPredmeta(p: Predmet) {
    if (p.naziv.split(' ').length === 1) {
      return p.naziv.substring(0, 3).toUpperCase();
    }
    return p.naziv.split(' ').map(rec => (!isNaN(rec as any) || rec.length !== 1) ? rec[0].toUpperCase() : '').join('');
  }


  h(str: string): number {
    return [...str].reduce((acc, char) => {
      return char.charCodeAt(0) + ((acc << 5) - acc);
    }, 0);
  }

  predmetColor: 'accent' | 'primary' = 'accent';

  backgroundColorHash(str: string) {
    const stringUniqueHash = this.h(str);
    const s = 1 / 25;
    let A;

    if (this.predmetColor === 'accent') {
      A = 19; // accent boja
      return `hsl(${A + s * (stringUniqueHash % 360 - A)}, 84%, 75%)`; // za accent
    }
    else {
      A = 229; // primary boja
      return `hsl(${A + s * (stringUniqueHash % 360 - A)}, 66%, 33%)`; // za primary
    }

    // console.log(A + s * (stringUniqueHash % 360 - A));
  }

  textColorHash(str: string) {
    const stringUniqueHash = this.h(str);
    const s = 1 / 35;
    let A;
    if (this.predmetColor === 'accent') {
      A = 19; // accent boja
      return `hsl(${A + s * (stringUniqueHash % 360 - A)}, 66%, 33%)`; // za accent
    }
    else {
      A = 229; // primary boja
      return `hsl(${A + s * (stringUniqueHash % 360 - A)}, 94%, 92%)`; // za primary
    }

    // console.log(A + s * (stringUniqueHash % 360 - A));

  }


  ngOnInit(): void {
  }

}

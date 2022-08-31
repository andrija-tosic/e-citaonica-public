import { Component, Input, OnInit } from '@angular/core';
import { Dodatak } from 'src/app/models/dodatak.model';

@Component({
  selector: 'dodatak-pregled',
  templateUrl: './dodatak-pregled.component.html',
  styleUrls: ['./dodatak-pregled.component.scss']
})
export class DodatakPregledComponent implements OnInit {
  @Input("dodaci") dodaci : Dodatak[] | undefined = [];

  constructor() { }

  ngOnInit(): void {
  }

}

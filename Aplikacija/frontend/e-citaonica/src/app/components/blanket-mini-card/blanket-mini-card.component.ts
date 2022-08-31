import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, BehaviorSubject } from 'rxjs';
import { Blanket } from 'src/app/models/blanket.model';
import { Predmet } from 'src/app/models/predmet.model';
import { PredmetService } from 'src/app/services/predmet.service';

@Component({
  selector: 'blanket-mini-card',
  templateUrl: './blanket-mini-card.component.html',
  styleUrls: ['./blanket-mini-card.component.scss']
})
export class BlanketMiniCardComponent implements OnInit {
  @Input('blanket') blanket: Blanket | undefined = undefined;
  @Input('showPredmet') showPredmet = false;

  constructor(public predmetService: PredmetService) { }

  ngOnInit(): void { }

}

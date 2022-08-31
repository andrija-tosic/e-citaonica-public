import { PredmetService } from 'src/app/services/predmet.service';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { Oblast } from './../models/oblast.model';
import { Injectable } from '@angular/core';
import { Predmet } from '../models/predmet.model';

@Injectable({
  providedIn: 'root'
})
export class OblastFilterService {

  public selektovaneOblasti$: BehaviorSubject<Oblast[]> = new BehaviorSubject<Oblast[]>([]);
  public selektovaniTipovi$: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);

  constructor(private predmetService: PredmetService) {
    this.predmetService.predmet$.subscribe((p) => this.selektovaneOblasti$.next([]));
  }
}

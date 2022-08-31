import { BehaviorSubject, Subject, take, Observable } from 'rxjs';
import { constants } from './../constants';
import { RezultatiPretrage } from "../types/rezultatiPretrage.type";
import { HttpClient } from '@angular/common/http';
import { User } from './../models/user.model';
import { Predmet } from './../models/predmet.model';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class PretragaService {

  public rezultati$: BehaviorSubject<RezultatiPretrage | null> = new BehaviorSubject<RezultatiPretrage | null>(null);
  public query: string = '';

  constructor(private http: HttpClient) {

  }

  pretrazi(query: string) {
    this.query = query;
    this.http.post<RezultatiPretrage>(`${environment.apiEndpoint}/pretraga`, `"${query}"`, constants.httpOptions).subscribe(rez => {
      this.rezultati$.next(rez);
    });

    return this.rezultati$;
  }


}

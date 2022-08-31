import { ApiMsg } from "../types/apiMsg.type";
import { environment } from 'src/environments/environment';
import { Modul } from 'src/app/models/modul.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IspitniRok } from '../models/ispitni-rok.model';
import { constants } from "../constants";

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  getModuli() {
    return this.http.get<Modul[]>(environment.apiEndpoint + "/moduli");
  }

  getIspitniRokovi() {
    return this.http.get<IspitniRok[]>(environment.apiEndpoint + "/rokovi");
  }

  // TODO: obrisiObavestenje

  obrisiObavestenje(id: number) {
    return this.http.delete<ApiMsg>(`${environment.apiEndpoint}/korisnici/obavestenja/${id}`, constants.httpOptionsBar);
  }
}

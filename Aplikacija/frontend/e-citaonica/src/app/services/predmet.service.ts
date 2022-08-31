import { AktivnostNiz } from './../types/aktivnostNiz.type';
import { moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { PredmetEdit } from './../models/DTOs/predmet.edit.dto';
import { constants } from '../constants';
import { ApiMsg } from "../types/apiMsg.type";
import { Observable, BehaviorSubject, ReplaySubject, tap, concat, catchError, of, first, map } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Predmet } from '../models/predmet.model';
import { IspitniRok } from '../models/ispitni-rok.model';
import { Blanket } from '../models/blanket.model';
import { Diskusija } from '../models/diskusija.model';
import { Zadatak } from '../models/zadatak.model';
import { DiskusijaAdd } from '../models/DTOs/diskusija.add.dto';
import { Student } from '../models/student.model';
import { Preporuka } from '../models/DTOs/preporuka.dto';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
}

@Injectable({
  providedIn: 'root'
})
export class PredmetService {
  ispitniRokoviFetched: boolean = false;

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);
  ispitniRokovi$: ReplaySubject<IspitniRok[]> = new ReplaySubject<IspitniRok[]>();
  public izabraniBlanket$: BehaviorSubject<Blanket | null> = new BehaviorSubject<Blanket | null>(null);
  public generisaniBlanket$: BehaviorSubject<Blanket | null> = new BehaviorSubject<Blanket | null>(null);

  constructor(private http: HttpClient) { }

  fetchPredmet(id: number) {
    // console.log("fetching predmet id=", id);
    this.http.get<Predmet>(`${environment.apiEndpoint}/predmeti/${id}`, httpOptions).subscribe({
      next: (p) => {
        // console.log(this.predmet$, p);
        this.predmet$.next(p);

        this.generisaniBlanket$.next(null);
        console.log('generisani -> null');

        this.izabraniBlanket$.next(null);
      },
      error: (err: ApiMsg) => console.log(err.msg)
    });
  }

  getIspitniRokovi() {
    if (!this.ispitniRokoviFetched) {
      this.http.get<IspitniRok[]>(`${environment.apiEndpoint}/rokovi`)
        .subscribe({
          next: (ir) => {
            this.ispitniRokovi$.next(ir);
            this.ispitniRokoviFetched = true;
          },
          error: (err: ApiMsg) => console.log(err.msg)
        })
    }
    return this.ispitniRokovi$;
  }

  getPredmet() {
    return this.predmet$;
  }

  dodajBlanket(data: any) {
    console.log(data);
    return this.http.post<Blanket>(`${environment.apiEndpoint}/blanketi/dodaj-blanket`, data, constants.httpOptions);
  }

  obrisiBlanket(id: number) {
    return this.http.delete<ApiMsg>(`${environment.apiEndpoint}/blanketi/${id}`);
  }

  addDiskusija(data: DiskusijaAdd) {
    return this.http.post<Diskusija>(`${environment.apiEndpoint}/diskusije/dodaj`, data, constants.httpOptions);
  }

  izmeniBlanket(data: any) {
    return this.http.put<Blanket>(`${environment.apiEndpoint}/blanketi/izmeni`, data, constants.httpOptions);
  }

  getBlanketi() {
    if (this.predmet$.value) {
      this.http.get<Blanket[]>(`${environment.apiEndpoint}/blanketi/predmet/${this.predmet$.value!.id}`).subscribe(blanketi => {
        this.predmet$.value!.blanketi = blanketi;
        console.log(blanketi)

        if (!this.izabraniBlanket$.value) {
          blanketi = blanketi.sort((b1, b2) => new Date(b2.datum).getTime() - new Date(b1.datum).getTime());

          if (blanketi.length > 0) {
            this.getFullBlanket(blanketi[0].id!).subscribe(blanket => {
              this.izabraniBlanket$.next(blanket);
            });
          }
        }
      });
    }
  }


  getFullBlanket(id: number) {
    return this.http.get<Blanket>(`${environment.apiEndpoint}/blanketi/${id}`);
  }



  getZadaci() {
    this.predmet$.subscribe(predmet => {
      if (predmet)
        this.http.get<Zadatak[]>(`${environment.apiEndpoint}/predmeti/zadaci/${predmet!.id}`).subscribe(zadaci => {

          // ne znam bolji nacin da sortiram zadatke po rednom broju oblasti

          let sortedZadaci: Zadatak[] = [];
          predmet?.oblasti?.sort(o => o.redniBr).forEach(o => {
            zadaci.forEach(z => {
              if (z.oblasti.map(o => o.id).includes(o.id))
                transferArrayItem(zadaci, sortedZadaci, zadaci.indexOf(z), sortedZadaci.length);
            });
          });

          predmet!.zadaci = sortedZadaci;
        });
    });
  }

  dodajPredmet(predmetEdit: PredmetEdit) {
    return this.http.post<Predmet>(`${environment.apiEndpoint}/predmeti`, predmetEdit, constants.httpOptions);
  }

  izmeniPredmet(predmetEdit: PredmetEdit) {
    return this.http.put<Predmet>(`${environment.apiEndpoint}/predmeti`, predmetEdit, constants.httpOptions);
  }

  obrisiPredmet(id: number) {
    return this.http.delete<ApiMsg>(`${environment.apiEndpoint}/predmeti/${id}`);
  }

  getDiskusije() {
    this.predmet$.subscribe(predmet => {
      if (predmet) {
        this.http.get<Diskusija[]>(`${environment.apiEndpoint}/diskusije/predmet/${predmet!.id}`).subscribe(diskusije => {
          console.log(diskusije);
          predmet!.diskusije = diskusije;
        });
      }
    });
  }

  getGenerisanBlanket(generisiBlanketModel: { oblastiIds: number[], tipovi: string[], predmetId: number }) {
    return this.http.post<Blanket>(`${environment.apiEndpoint}/predmeti/generisi-blanket`, generisiBlanketModel, constants.httpOptions);
  }

  getNajaktivnijiStudenti(id: number) {
    return this.http.get<AktivnostNiz>(`${environment.apiEndpoint}/predmeti/najaktivniji-studenti/${id}`);
  }

  getPreporuke(id: number, query: string = '', exclude: number[] = []) {
    let excparam: string = exclude.reduce((prev, cur) => prev + cur + 'i', '');
    let params = new HttpParams().set("query", query).set("exclude", excparam);
    return this.http.get<Preporuka[]>(`${environment.apiEndpoint}/predmeti/preporuke/${id}`, { params: params });
  }
}

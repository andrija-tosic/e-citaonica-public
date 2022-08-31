import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { constants } from '../constants';
import { Diskusija } from '../models/diskusija.model';
import { DiskusijaAdd } from '../models/DTOs/diskusija.add.dto';
import { DiskusijaEditRes } from '../models/DTOs/diskusija.edit.res';
import { Komentar } from '../models/komentar.model';
import { ApiMsg } from '../types/apiMsg.type';
import { PredmetService } from './predmet.service';

@Injectable({
  providedIn: 'root'
})
export class DiskusijaService {

  constructor(private predmetService: PredmetService, private http: HttpClient) {
  }

  getDiskusija(id: number, depth: number = 5) {
    return this.http.get<Diskusija>(`${environment.apiEndpoint}/diskusije/${id}/${depth}`);
  }

  updateDiskusija(diskusija: DiskusijaAdd) {
    return this.http.put<DiskusijaEditRes>(`${environment.apiEndpoint}/diskusije/izmeni-diskusiju`, diskusija, constants.httpOptions);
  }

  deleteDiskusija(diskusijaId: number) {
    return this.http.delete<ApiMsg>(`${environment.apiEndpoint}/diskusije/${diskusijaId}`);
  }

  postKomentar(komentar: Komentar) {
    return this.http.post<Komentar>(`${environment.apiEndpoint}/komentari`, komentar, constants.httpOptions);
  }

  zahvaliToggle(objavaId: number) {
    return this.http.put<ApiMsg>(`${environment.apiEndpoint}/zahvalnica/zahvali`, objavaId, constants.httpOptionsBar);
  }

  zapratiToggle(diskusijaId: number) {
    return this.http.put<ApiMsg>(`${environment.apiEndpoint}/diskusije/zaprati`, diskusijaId, constants.httpOptionsBar);
  }

  updateKomentar(komentar: Komentar) {
    return this.http.put<Komentar>(`${environment.apiEndpoint}/komentari/izmeni-komentar`, komentar, constants.httpOptions);
  }

  deleteKomentar(komentarId: number) {
    return this.http.delete<ApiMsg>(`${environment.apiEndpoint}/komentari/${komentarId}`);
  }

  tacnostToggle(komentarId: number) {
    return this.http.put<ApiMsg>(`${environment.apiEndpoint}/komentari/potvrdjivanje-tacnosti/`, komentarId, constants.httpOptions);
  }

  prihvatiToggle(komentarId: number) {
    return this.http.put<ApiMsg>(`${environment.apiEndpoint}/komentari/prihvati-odgovor-toggle/`, komentarId, constants.httpOptions);
  }

  getDiskusijeSaResenjemZadatka(zadatakId: number) {
    return this.http.get<Diskusija[]>(`${environment.apiEndpoint}/diskusije/resenja-zadatka/${zadatakId}`);
  }

  getDiskusijeZadatka(zadatakId: number) {
    return this.http.get<Diskusija[]>(`${environment.apiEndpoint}/diskusije/zadatak/${zadatakId}`);
  }

  prijaviObjavu(objavaId: number, tekst: string) {
    return this.http.post<ApiMsg>(`${environment.apiEndpoint}/diskusije/prijavi`, { objavaId: objavaId, sadrzaj: tekst }, constants.httpOptions);
  }

  arhiviraj(diskusijaId: number) {
    return this.http.put(`${environment.apiEndpoint}/diskusije/arhiviraj`, diskusijaId, constants.httpOptions);
  }
}

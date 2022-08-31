import { constants } from './../constants';
import { HttpClient, HttpEvent, HttpEventType, HttpProgressEvent, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, scan } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/user.model';
import { calculateState, Upload } from './service.helper';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  constructor(private http: HttpClient) { }

  uploadImage(image: any) {
    const formData = new FormData();
    formData.append('file', image);
    return this.http.post(`${environment.apiEndpoint}/files/upload/image`, formData)
      .pipe(map((res: any) => {
        return res.path;
      }));
  }

  uploadFile(file: any) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${environment.apiEndpoint}/files/upload`, formData)
      .pipe(map((res: any) => {
        return res.path;
      }));
  }

  uploadFileWithProgress(file: any) {
    const formData = new FormData();
    formData.append('file', file);

    const initialState: Upload = { state: 'PENDING', progress: 0 }

    return this.http.post(`${environment.apiEndpoint}/files/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(scan(calculateState, initialState));
  }

  deleteUserImage(user: User) {
    console.log("brisanje za user", user.id);
    return this.http.put<{ msg: string }>(`${environment.apiEndpoint}/korisnici/obrisi-sliku/`, user.id);

  }

  changeUserImage(image: any, user: User) {

    const formData = new FormData();
    formData.append('file', image);

    return this.http.post<{ path: string }>(`${environment.apiEndpoint}/korisnici/postavi-sliku/${user.id}`, formData);

  }


  changeUserImageWithProgress(image: any, brisanje: boolean, user: User) {

    const initialState: Upload = { state: 'PENDING', progress: 0 }

    if (brisanje) {
      console.log("brisanje za user", user.id);
      return this.http.put(`${environment.apiEndpoint}/korisnici/obrisi-sliku/`, user.id, {
        reportProgress: true,
        observe: 'events'
      })
        .pipe(scan(calculateState, initialState));
    }
    else {
      const formData = new FormData();
      formData.append('file', image);

      return this.http.post(`${environment.apiEndpoint}/korisnici/postavi-sliku/${user.id}`, formData, {
        reportProgress: true,
        observe: 'events'
      })
        .pipe(scan(calculateState, initialState));
    }
  }
}

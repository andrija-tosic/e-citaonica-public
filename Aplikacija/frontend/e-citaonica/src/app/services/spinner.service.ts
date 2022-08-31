import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SpinnerService {

  private count = 0;
  private spinner = new BehaviorSubject<boolean>(false);

  constructor() { }

  getSpinnerObserver() : Observable<boolean> {
    return this.spinner;
  }

  requestStarted() {

    // timeout da ne flickeruje spinner
    setTimeout(() => {
      if (this.count++ == 0) this.spinner.next(true);
    }, 500);
  }

  requestEnded() {
    if (--this.count == 0) this.spinner.next(false);
  }
}

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { SpinnerService } from './spinner.service';

export const SpinnerSkipHeader = 'skip-spinner';

@Injectable({
  providedIn: 'root'
})
export class SpinnerInterceptorService implements HttpInterceptor {

  constructor(private spinnerService : SpinnerService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.headers.has(SpinnerSkipHeader)) {
      const headers = request.headers.delete(SpinnerSkipHeader);
      return next.handle(request.clone({ headers }));
    }

    this.spinnerService.requestStarted();

    return next.handle(request)
    .pipe(
      finalize(() => {
        this.spinnerService.requestEnded();
      })
    );
  }
}

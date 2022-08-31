import { HttpContext, HttpHeaders } from '@angular/common/http';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { NGX_LOADING_BAR_IGNORED } from '@ngx-loading-bar/http-client';

export const title = 'e-citaonica';

export const tipoviBlanketa: string[] = [
  'pismeni',
  'teorijski',
  'kombinovani'
];

export const constants = {
  snackbarPosition: {
    verticalPosition: 'bottom',
    horizontalPosition: 'right',
  } as MatSnackBarConfig<any>,

  httpOptions: {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  },

  httpBar: {
    context: new HttpContext().set(NGX_LOADING_BAR_IGNORED, true)
  },

  httpOptionsBar: {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
    context: new HttpContext().set(NGX_LOADING_BAR_IGNORED, true)
  }
};

export const AUTORIZACIJA_DISABLED: boolean = false;

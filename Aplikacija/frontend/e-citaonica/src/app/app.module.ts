import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeSr from '@angular/common/locales/sr-Latn-ME';
registerLocaleData(localeSr);

import { PendingChangesGuard } from './guards/pending-changes.guard';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { BrowserModule } from '@angular/platform-browser';
import { AngularMaterialModule } from './angular-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  IPublicClientApplication,
  PublicClientApplication,
} from '@azure/msal-browser';
import { MsalService, MSAL_INSTANCE } from '@azure/msal-angular';
import { environment } from 'src/environments/environment';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { LoginComponent } from './pages/login/login.component';
import { AppComponent } from './app.component';
import { AuthService } from './services/auth.service';
import { RegisterComponent } from './pages/register/register.component';
import { SsoRegisterComponent } from './pages/sso-register/sso-register.component';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { UserComponent } from './pages/user/user.component';
import { HttpInterceptorService } from './services/http-interceptor.service';
import { PredmetComponent } from './pages/predmet/predmet.component';
import { HomeComponent } from './pages/home/home.component';
import { MainNavComponent } from './components/main-nav/main-nav.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { JwtModule, JwtModuleOptions } from '@auth0/angular-jwt';
import { NavigacijaComponent } from './pages/navigacija/navigacija.component';
import { PregledComponent } from './pages/pregled/pregled.component';
import { BlanketiComponent } from './pages/blanketi/blanketi.component';
import { ZadaciComponent } from './pages/zadaci/zadaci.component';
import { DiskusijeComponent } from './pages/diskusije/diskusije.component';
import { QuillModule } from 'ngx-quill';
import { QuillWrapperComponent } from './components/quill-wrapper/quill-wrapper.component';
import { ZadatakComponent } from './components/zadatak/zadatak.component';
import { PretragaComponent } from './pages/pretraga/pretraga.component';
import { FormsModule } from '@angular/forms';
import { DodatakPregledComponent } from './components/dodatak-pregled/dodatak-pregled.component';
import { UploadDodatakComponent } from './components/upload-dodatak/upload-dodatak.component';
import { UserCardComponent } from './components/user-card/user-card.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { DeleteConfirmationDialogComponent } from './components/delete-confirmation-dialog/delete-confirmation-dialog.component'
import { DiskusijaPreviewComponent } from './components/diskusija-preview/diskusija-preview.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { IzmenaSlikeDialogComponent } from './components/izmena-slike-dialog/izmena-slike-dialog.component';
import { BlanketSubmitComponent } from './pages/blanket-submit/blanket-submit.component';
import { DiskusijaSubmitComponent } from './pages/diskusija-submit/diskusija-submit.component';
import { SpinnerInterceptorService } from './services/spinner-interceptor.service';
import { BlanketMiniCardComponent } from './components/blanket-mini-card/blanket-mini-card.component';
import { BlanketPreviewComponent } from './components/blanket-preview/blanket-preview.component';
import { SanitizeHtmlPipe } from './pipes/sanitize-html.pipe';
import { BlanketPreviewHandsetComponent } from './components/blanket-preview-handset/blanket-preview-handset.component';
import { CommentComponent } from './components/comment/comment.component';
import { DiskusijaComponent } from './pages/diskusija/diskusija.component';
import { CommentFormComponent } from './components/comment-form/comment-form.component';
import { BlanketFormComponent } from './components/blanket-form/blanket-form.component';
import { OblastFilterComponent } from './components/oblast-filter/oblast-filter.component';
import { PromenaLozinkeDialogComponent } from './components/promena-lozinke-dialog/promena-lozinke-dialog.component';
import { DiskusijaFormComponent } from './components/diskusija-form/diskusija-form.component';
// for HttpClient import:
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
// for Router import:
// import { LoadingBarRouterModule } from '@ngx-loading-bar/router';

// for Core import:
import { LoadingBarModule } from '@ngx-loading-bar/core';
import { PredmetFormComponent } from './components/predmet-form/predmet-form.component';
import { PredmetSubmitComponent } from './pages/predmet-submit/predmet-submit.component';
import { NavSplitComponent } from './components/nav-split/nav-split.component';
import { VezbanjeComponent } from './components/vezbanje/vezbanje.component';
import { PredmetCardComponent } from './components/predmet-card/predmet-card.component';
import { PrijaviDialogComponent } from './components/prijavi-dialog/prijavi-dialog.component';
import { DiskusijeResenjaZadatkaComponent } from './components/diskusije-resenja-zadatka/diskusije-resenja-zadatka.component';
import { OdaberiKolegeDialogComponent } from './components/odaberi-kolege-dialog/odaberi-kolege-dialog.component';
import { PreporukaCardComponent } from './components/preporuka-card/preporuka-card.component';
import { PrettyDate } from './pipes/pretty-date.pipe';
import { PredmetIconComponent } from './components/predmet-icon/predmet-icon.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { ObavestenjaComponent } from './components/obavestenja/obavestenja.component';

function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication(environment.msalConfig);
}

export function tokenGetter() {
  return localStorage.getItem('access_token');
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    SsoRegisterComponent,
    ConfirmEmailComponent,
    SpinnerComponent,
    HomeComponent,
    MainNavComponent,
    UserComponent,
    PredmetComponent,
    NavigacijaComponent,
    PregledComponent,
    BlanketiComponent,
    ZadaciComponent,
    DiskusijeComponent,
    QuillWrapperComponent,
    ZadatakComponent,
    PretragaComponent,
    DodatakPregledComponent,
    UploadDodatakComponent,
    UserCardComponent,
    DeleteConfirmationDialogComponent,
    DiskusijeComponent,
    DiskusijaPreviewComponent,
    IzmenaSlikeDialogComponent,
    BlanketSubmitComponent,
    DiskusijaSubmitComponent,
    BlanketMiniCardComponent,
    BlanketPreviewComponent,
    SanitizeHtmlPipe,
    PrettyDate,
    BlanketPreviewHandsetComponent,
    CommentComponent,
    DiskusijaComponent,
    CommentFormComponent,
    BlanketFormComponent,
    OblastFilterComponent,
    PromenaLozinkeDialogComponent,
    DiskusijaFormComponent,
    PredmetFormComponent,
    PredmetSubmitComponent,
    NavSplitComponent,
    VezbanjeComponent,
    PredmetCardComponent,
    PrijaviDialogComponent,
    DiskusijeResenjaZadatkaComponent,
    OdaberiKolegeDialogComponent,
    PreporukaCardComponent,
    PredmetIconComponent,
    ObavestenjaComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    AngularMaterialModule,
    HttpClientModule,
    LayoutModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    InfiniteScrollModule,
    QuillModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
      },
    } as JwtModuleOptions),
    FormsModule,
    DragDropModule,
    FlexLayoutModule,
    LoadingBarHttpClientModule,
    LoadingBarModule,
    NgxSkeletonLoaderModule
  ],
  providers: [
    { provide: LOCALE_ID, useValue: "sr-Latn-ME" },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SpinnerInterceptorService,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpInterceptorService,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    AuthService,
    MsalService,
    PendingChangesGuard,
  ],
  bootstrap: [AppComponent],
  entryComponents: [BlanketPreviewHandsetComponent]
})
export class AppModule { }

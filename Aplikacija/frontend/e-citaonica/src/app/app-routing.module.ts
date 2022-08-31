import { ObavestenjaComponent } from './components/obavestenja/obavestenja.component';
import { DiskusijeResenjaZadatkaComponent } from './components/diskusije-resenja-zadatka/diskusije-resenja-zadatka.component';
import { VezbanjeComponent } from './components/vezbanje/vezbanje.component';
import { PredmetSubmitComponent } from './pages/predmet-submit/predmet-submit.component';
import { PretragaComponent } from './pages/pretraga/pretraga.component';
import { NavigacijaComponent } from './pages/navigacija/navigacija.component';
import { AuthGuardService } from './services/auth-guard.service';
import { UserComponent } from './pages/user/user.component';
import { SsoRegisterComponent } from './pages/sso-register/sso-register.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { PredmetComponent } from './pages/predmet/predmet.component';
import { HomeComponent } from './pages/home/home.component';
import { NegateAuthGuardService } from './services/negate-auth-guard.service';
import { PregledComponent } from './pages/pregled/pregled.component';
import { ZadaciComponent } from './pages/zadaci/zadaci.component';
import { BlanketiComponent } from './pages/blanketi/blanketi.component';
import { DiskusijeComponent } from './pages/diskusije/diskusije.component';
import { DiskusijaSubmitComponent } from './pages/diskusija-submit/diskusija-submit.component';
import { BlanketSubmitComponent } from './pages/blanket-submit/blanket-submit.component';
import { DiskusijaComponent } from './pages/diskusija/diskusija.component';
import { PendingChangesGuard } from './guards/pending-changes.guard';
import { NavSplitComponent } from './components/nav-split/nav-split.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
const routes: Routes = [
  { path: '', redirectTo: 'pocetna', pathMatch: 'full' },
  {
    path: '',
    component: NavigacijaComponent,
    canActivate: [AuthGuardService],
    children: [
      { path: 'pocetna', component: HomeComponent },
      { path: 'korisnik/:id', component: UserComponent },
      {
        path: 'predmet/:id',
        redirectTo: 'predmet/:id/pregled'
      },
      {
        path: 'predmet/:id',
        component: PredmetComponent,
        children: [
          {
            path: 'pregled',
            component: PregledComponent,
            canDeactivate: [PendingChangesGuard]
          },
          {
            path: 'zadaci',
            component: ZadaciComponent
          },
          {
            path: 'blanketi',
            component: NavSplitComponent,
            children: [
              {
                path: '',
                component: BlanketiComponent
              },
              {
                path: 'vezbanje', component: VezbanjeComponent
              },
              {
                path: 'submit',
                component: BlanketSubmitComponent,
                canDeactivate: [PendingChangesGuard]
              },
            ]
          },

          {
            path: 'diskusije',
            component: NavSplitComponent,
            children: [
              {
                path: 'submit',
                component: DiskusijaSubmitComponent,
                canDeactivate: [PendingChangesGuard]
              },
              {
                path: 'resenja-zadatka/:id',
                component: DiskusijeResenjaZadatkaComponent
              },
              {
                path: 'zadatak/:id',
                component: DiskusijeResenjaZadatkaComponent
              },
              {
                path: ':id',
                component: DiskusijaComponent
              },
              {
                path: '',
                component: DiskusijeComponent
              }
            ]
          }

        ]
      },
      { path: 'pretraga', component: PretragaComponent },
      { path: 'obavestenja', component: ObavestenjaComponent },
      { path: 'dodaj-predmet', component: PredmetSubmitComponent, canDeactivate: [PendingChangesGuard] }
    ]
  },
  { path: 'login', component: LoginComponent, canActivate: [NegateAuthGuardService] },
  { path: 'register', component: RegisterComponent, canActivate: [NegateAuthGuardService] },
  { path: 'sso-register', component: SsoRegisterComponent, canActivate: [NegateAuthGuardService] },
  { path: 'confirm-email', component: ConfirmEmailComponent, canActivate: [NegateAuthGuardService] },
  { path: '**', redirectTo: '', component: HomeComponent, canActivate: [AuthGuardService] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

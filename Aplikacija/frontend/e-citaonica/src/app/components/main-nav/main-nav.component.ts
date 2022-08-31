import { Predmet } from './../../models/predmet.model';
import { constants, AUTORIZACIJA_DISABLED } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiService } from './../../services/api.service';
import { UserService } from './../../services/user.service';
import { PredmetService } from './../../services/predmet.service';
import { ChangeDetectorRef, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable, BehaviorSubject, Subject, ReplaySubject, first } from 'rxjs';
import { map, share, shareReplay, switchMap } from 'rxjs/operators';
import { User } from 'src/app/models/user.model';
import { Student } from 'src/app/models/student.model';
import { Obavestenje } from 'src/app/models/obavestenje.model';
import { AuthService } from 'src/app/services/auth.service';
import { NavigationStart, Router } from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';
import { UserBasic } from 'src/app/models/user-basic.model';

@Component({
  selector: 'main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1200px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  user$: ReplaySubject<User> = new ReplaySubject<User>();
  user: UserBasic;
  obavestenja: Obavestenje[];
  obavestenjaInterval: ReturnType<typeof setInterval>;
  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  jeMobilni: boolean = false; // potrebno, inace ima bug, i detektuje kao da jeste i da nije handset

  predmetColor: 'accent' | 'primary' = 'accent';

  @ViewChild('sideNav') sidenav: MatSidenav;

  constructor(
    private breakpointObserver: BreakpointObserver,
    public authService: AuthService,
    public router: Router,
    private userService: UserService,
    private cdRef: ChangeDetectorRef,
    private eRef: ElementRef,
    public predmetService: PredmetService,
    private apiService: ApiService,
    private _snackbar: MatSnackBar) {

    this.authService.getUserObserver().subscribe({
      next: u => {
        this.user = u!;
        this.user$.next(u as User);
        this.authService.getObavestenja().subscribe(o => {
          u!.obavestenja = o;
          this.user.obavestenja = o;
          this.user$.next(this.user as User);
        });
        //clearInterval(this.obavestenjaInterval);
        // this.obavestenjaInterval = setInterval(_ => {
        //   this.authService.getObavestenja().subscribe(o => {
        //     this.user.obavestenja = o;
        //     this.user$.next(this.user as User);
        //   });
        // }, 30000);
      }
    });

    this.userService.fetchPraceniPredmeti();

    this.isHandset$.subscribe(b => this.jeMobilni = b);
  }

  scrollToTop() {
    document.querySelector('.mat-sidenav-content')!.scrollTo(
      {
        top: 0,
        left: 0,
        behavior: 'smooth'
      }
    );
  }

  toggleSideNav() {
    this.sidenav.toggle();
    localStorage.setItem('sidenav_open', JSON.stringify(this.sidenav.opened));
  }

  redirekcija(url: string) {
    this.router.navigate([url + "/" + this.user.id]);
  }

  ngAfterViewInit() {
    this.cdRef.detectChanges();

    // close sidenav on routing
    this.router.events.forEach((event) => {
      if (event instanceof NavigationStart) {
        if (this.jeMobilni) {
          this.sidenav.close();
        }
        else {
          // console.log("nije handset");
        }
      }
    });

    this.isHandset$.pipe(first()).subscribe(isHandset => {
      if (!isHandset) {
        const open = localStorage.getItem('sidenav_open');

        if (open !== null) {
          const sidenavOpen: boolean = JSON.parse(open);
          if (sidenavOpen) {
            this.sidenav.open();
            this.cdRef.detectChanges();

          }
          else {
            this.sidenav.close();
            this.cdRef.detectChanges();

          }
        }
      }
    })

  }

  imaPredmetaUGodini(godina: number) {
    return this.user.praceniPredmeti?.some(p => p.godina === godina);
  }

  imaPredmetaUSemestruIGodini(semestar: number, godina: number) {
    return this.user.praceniPredmeti?.some(p => p.semestar === semestar && p.godina === godina);
  }

  obrisiObavestenje(obavestenje: Obavestenje) {
    ((obavestenje: Obavestenje) => {
      this.user.obavestenja = this.user.obavestenja?.filter(o => o.id != obavestenje.id);
      this.apiService.obrisiObavestenje(obavestenje.id).subscribe({
        next: _ => {
          this.user$.next(this.user as User);
        },
        error: _ => {
          this.user.obavestenja?.push(obavestenje);
          this.user.obavestenja?.sort((a, b) => new Date(a.datumIVreme).getTime() - new Date(b.datumIVreme).getTime())
        }
      })
    })(obavestenje)
  }

  pregledObavestenja(obavestenje: Obavestenje) {
    this.router.navigate([`/predmet/${obavestenje.predmetId}/diskusije/${obavestenje.diskusijaId}`], { state: { scrollAnchor: `${obavestenje.objavaId}` } });
  }
}

import { Title } from '@angular/platform-browser';
import { constants } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PromenaLozinkeDialogComponent } from './../../components/promena-lozinke-dialog/promena-lozinke-dialog.component';
import { IzmenaSlikeDialogComponent } from './../../components/izmena-slike-dialog/izmena-slike-dialog.component';
import { AuthService } from './../../services/auth.service';
import { UserService } from './../../services/user.service';
import { concatMap, first, map, mergeMap, Observable, ObservableInput, of, switchMap, tap, ReplaySubject, subscribeOn, shareReplay, BehaviorSubject, Subscription, share } from 'rxjs';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { User } from './../../models/user.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UserBasic } from 'src/app/models/user-basic.model';
import { MatDialog } from '@angular/material/dialog';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Diskusija } from 'src/app/models/diskusija.model';
import { Objava } from 'src/app/models/objava.model';
import { Komentar } from 'src/app/models/komentar.model';
import { Profiler } from 'inspector';

@Component({
  selector: 'user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

  user$: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );
  scrolled$: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);
  refresh$: Observable<any> | null = null;

  objave: Array<Komentar | Diskusija> = []; 

  loggedUser: Observable<UserBasic | null> = new Observable(); // samo referenca na authService user Observable
  private scrollSubscription = new Subscription();

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    public authService: AuthService,
    public dialog: MatDialog,
    private snackbar: MatSnackBar,
    private breakpointObserver: BreakpointObserver,
    private title: Title
  ) {
    let id: number;
    this.route.paramMap.subscribe((params: ParamMap) => {
      id = +params.get('id')!

      // this.userService.fetchUser(id).subscribe(u => {
      //   this.userService.getDiskusijeKorisnika(u.id!).subscribe(diskusije => {
      //     u.diskusije = diskusije;
      //     this.userService.user$.next(u);
      //     this.user$.next(u);
      //   });
      // })

      this.userService.fetchUser(id).subscribe(u => {
        this.title.setTitle(`${u.ime} • e-Čitaonica`);

        this.userService.user$.next(u);
        this.user$.next(u);
      })

      this.scrollSubscription = this.scrolled$.subscribe(_ => {
        this.refresh$ = this.userService.getObjaveKorisnika(id, { 
          komod: this.lastObjava(false)?.datumKreiranja,
          disod: this.lastObjava(true)?.datumKreiranja
        }).pipe(share());
        this.refresh$.subscribe((objave) => {
          if (objave.length == 0)
            this.scrollSubscription.unsubscribe();
          this.objave = this.objave?.concat(objave) || objave;
          this.refresh$ = null;
        });
      })


      this.loggedUser = this.authService.getUserObserver();
    });
  }

  lastObjava(isDiskusija: boolean) : Objava | null {
    for (let i = this.objave.length - 1; i >= 0; i--) {
      if (isDiskusija && this.isDiskusija(this.objave[i])) {
        return this.objave[i];
      }
      else if (!isDiskusija && !this.isDiskusija(this.objave[i])) {
        return this.objave[i];
      }
    }
    return null;
  }

  isDiskusija(objava: any): boolean {
    return 'zavrsena' in objava;
  }

  izmenaSlike() {

    this.loggedUser.pipe(first())
      .subscribe(user => {
        const dialogRef = this.dialog.open(IzmenaSlikeDialogComponent, {
          data: user
        });
        dialogRef.afterClosed().subscribe(result => {
          console.log(`Dialog result: ${result}`);
          if (result === null || result === undefined)
            return;

          user!.slikaURL = result;

          this.user$.pipe(first())
            .subscribe(u => u!.slikaURL = result);

          this.authService.updateUser(user as User);
        });
      });
  }

  promenaLozinke() {
    const dialogRef = this.dialog.open(PromenaLozinkeDialogComponent);
    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
      if (result === null || result === undefined)
        return;

      this.snackbar.open(result, 'OK', { ...constants.snackbarPosition, duration: 3000 });
    })
  }

  ngOnInit(): void {
  }

  onScrollDown() {
    console.log("scrolled");
    this.scrolled$.next();
  }
}

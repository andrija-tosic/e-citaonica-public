import { Title } from '@angular/platform-browser';
import { PredmetService } from './../../services/predmet.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Blanket } from './../../models/blanket.model';
import { Diskusija } from './../../models/diskusija.model';
import { BehaviorSubject, combineLatest, combineLatestWith, map, Observable, share, shareReplay, Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { ApiMsg } from "../../types/apiMsg.type";
import { UserService } from './../../services/user.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Predmet } from 'src/app/models/predmet.model';
import { User } from 'src/app/models/user.model';
import { UserBasic } from 'src/app/models/user-basic.model';
import { Dodatak } from 'src/app/models/dodatak.model';
import { RouterLinkActive } from '@angular/router';
import { ThrottlingUtils } from '@azure/msal-common';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private predmetService: PredmetService,
    private breakpointObserver: BreakpointObserver,
    private title: Title) { }

  subscriptions: Subscription[] = [];
  skipSub: Subscription;

  refresh$: Observable<Diskusija[]> | null = null;
  diskusije$: BehaviorSubject<Diskusija[] | null> = new BehaviorSubject<Diskusija[] | null>(null);
  blanketi$: BehaviorSubject<Blanket[] | null> = new BehaviorSubject<Blanket[] | null>(null);
  scrolled$: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);
  loading: number = 0;
  caughtUp: boolean = false;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1300px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );


  ngOnInit(): void {
    this.title.setTitle("e-Čitaonica");

    this.predmetService.izabraniBlanket$.next(null);

    this.subscriptions.push(this.authService.getUserObserver().subscribe(user => {
      if (user && user.praceniPredmeti) {
        this.userService.getPreporuceniBlanketi(user.praceniPredmeti.map(p => p.id)).subscribe(blanketi => {
          this.blanketi$.next(blanketi);
        })
      }
    })
    )

    this.skipSub = this.scrolled$.subscribe(_ => {
      if (!this.refresh$) {
        this.loading++;

        const last = this.diskusije$?.value?.[this.diskusije$.value?.length - 1];
        this.refresh$ = this.userService
          .getPreporuceneDiskusije({ dated : last?.datumKreiranja })
          .pipe(share());
        this.refresh$.subscribe({
          next: (diskusije) => {
            if (diskusije.length == 0) {
              this.skipSub.unsubscribe();
              this.caughtUp = true;
            }
            this.diskusije$.next(this.diskusije$.value?.concat(diskusije) || diskusije);
            this.loading--;
          },
          error: _ => {
            this.loading--;
          },
          complete: () => {
            this.refresh$ = null;
          }
        });
      }
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  scrollLeft() {

  }
  scrollRight() {

  }

  onScrollDown() {
    this.scrolled$.next();
  }
}

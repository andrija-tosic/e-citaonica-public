import { BreakpointObserver } from '@angular/cdk/layout';
import { map, Observable, shareReplay } from 'rxjs';
import { Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { title } from './constants';
import { BrowserRefreshService } from './services/browser-refresh.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = title;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );


  constructor(
    private AuthService: AuthService,
    private browserRefresh: BrowserRefreshService,
    private router: Router,
    private breakpointObserver: BreakpointObserver) {
    browserRefresh.subscribe();
  }

  setLoadingBarTopMargin() {
    return !['/login', '/register', 'sso-register', 'confirm-email'].includes(this.router.url);
  }

  ngOnDestroy(): void {
    this.browserRefresh.unsubscribe();
  }

  ngOnInit(): void {
    this.AuthService.autoLogin();
  }
}

import { Injectable } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BrowserRefreshService {
  private browserRefreshed = false;
  private subscription : Subscription;
  constructor(private router: Router) { 

  }

  subscribe() {
    this.subscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.browserRefreshed = !this.router.navigated;
      }
    })
  }

  unsubscribe() {
    this.subscription.unsubscribe();
  }

  get refreshed() : boolean {
    return this.browserRefreshed;
  }
}

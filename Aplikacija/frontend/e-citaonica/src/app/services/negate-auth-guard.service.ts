import { AuthService } from 'src/app/services/auth.service';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthGuardService } from './auth-guard.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NegateAuthGuardService implements CanActivate {

  constructor(private auth: AuthService, private router: Router) { 
  }
    canActivate(): boolean {
      if (this.auth.isAuthenticated()){
        this.router.navigate(['/pocetna']);
        return false;
      }
      return true;
    }
}

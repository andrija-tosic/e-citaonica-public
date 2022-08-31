import { UserBasic } from './../../models/user-basic.model';
import { AuthService } from 'src/app/services/auth.service';
import { User } from './../../models/user.model';
import { ApiService } from './../../services/api.service';
import { Router } from '@angular/router';
import { Obavestenje } from './../../models/obavestenje.model';
import { UserService } from './../../services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'obavestenja',
  templateUrl: './obavestenja.component.html',
  styleUrls: ['./obavestenja.component.scss']
})
export class ObavestenjaComponent implements OnInit {

  user: UserBasic;

  periodi = [24, 168, 169];

  constructor(public authService: AuthService,
    private router: Router,
    private apiService: ApiService) { }

  ngOnInit(): void {
    this.authService.getUserObserver().subscribe(u => { if (u) this.user = u });

    // this.authService.getObavestenja().subscribe(o => {
    //   this.user.obavestenja = o;
    // this.user$.next(this.user as User);
    // });
  }

  prosloSati(d: Date | string) {
    return Math.abs((new Date()).getTime() - (new Date(d)).getTime()) / 3600000;
  }

  imaObavestenjaZaPeriod(period1: number, period2: number = Number.MAX_VALUE) {

    return this.user.obavestenja?.some(o =>
      this.prosloSati(o.datumIVreme) >= period1 && this.prosloSati(o.datumIVreme) <= period2);
  }

  pregledObavestenja(obavestenje: Obavestenje) {
    this.router.navigate([`/predmet/${obavestenje.predmetId}/diskusije/${obavestenje.diskusijaId}`], { state: { scrollAnchor: `${obavestenje.objavaId}` } });
  }

  obrisiObavestenje(obavestenje: Obavestenje) {
    ((obavestenje: Obavestenje) => {
      this.user.obavestenja = this.user.obavestenja?.filter(o => o.id != obavestenje.id);
      this.apiService.obrisiObavestenje(obavestenje.id).subscribe({
        next: _ => {
          // this.user$.next(this.user as User);
        },
        error: _ => {
          this.user.obavestenja?.push(obavestenje);
          this.user.obavestenja?.sort((a, b) => new Date(a.datumIVreme).getTime() - new Date(b.datumIVreme).getTime())
        }
      })
    })(obavestenje)
  }


}

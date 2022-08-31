import { AuthService } from './../../services/auth.service';
import { Zadatak } from './../../models/zadatak.model';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Predmet } from 'src/app/models/predmet.model';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, Observable, shareReplay } from 'rxjs';

@Component({
  selector: 'zadatak',
  templateUrl: './zadatak.component.html',
  styleUrls: ['./zadatak.component.scss']
})
export class ZadatakComponent implements OnInit {
  @Input('hasPokreni') hasPokreni: boolean = true;
  @Input() zadatak: Zadatak;
  @Input() predmet: Predmet;

  @Input('sliceLength') sliceLength: number = Number.MAX_VALUE;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(
    public authService: AuthService, 
    private router: Router, 
    private route: ActivatedRoute, 
    private breakpointObserver: BreakpointObserver,
    ) { }

  ngOnInit(): void {
  }

  onPokreniDiskusiju() {
    if (this.router.url.includes('vezbanje')) {
      this.router.navigate([`/predmet/${this.predmet.id}/diskusije/submit`], { state: this.zadatak });
    }
    else {
      this.router.navigate([`/predmet/${this.predmet.id}/diskusije/submit`], { state: this.zadatak });
    }
  }

  onVidiResenja() {
    if (this.router.url.includes('vezbanje')) {
      this.router.navigate([`/predmet/${this.predmet.id}/diskusije/resenja-zadatka/${this.zadatak.id}`, { state: this.zadatak }]);

    }
    else {
      console.log(this.zadatak.id)
      this.router.navigate([`predmet/${this.predmet.id}/diskusije/resenja-zadatka/${this.zadatak.id}`, { state: this.zadatak }]);
    }
  }

  onVidiDiskusije() {
    console.log('ee')
    // this.router.navigate(['/pocetna']);
    if (this.router.url.includes('vezbanje')) {
      this.router.navigate([`/predmet/${this.predmet.id}/diskusije/zadatak/${this.zadatak.id}`]);

    }
    else {
      // console.log(this.zadatak.id)
      this.router.navigate([`/predmet/${this.predmet.id}/diskusije/zadatak/${this.zadatak.id}`]);
    }
  }
}

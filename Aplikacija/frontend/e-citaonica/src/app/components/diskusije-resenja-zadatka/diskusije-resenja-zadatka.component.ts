import { Diskusija } from 'src/app/models/diskusija.model';
import { Router, ActivatedRoute } from '@angular/router';
import { Zadatak } from 'src/app/models/zadatak.model';
import { DiskusijaService } from 'src/app/services/diskusija.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, OnInit } from '@angular/core';
import { map, shareReplay, BehaviorSubject, switchMap } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { Predmet } from 'src/app/models/predmet.model';
import { PredmetService } from 'src/app/services/predmet.service';

@Component({
  selector: 'diskusije-resenja-zadatka',
  templateUrl: './diskusije-resenja-zadatka.component.html',
  styleUrls: ['./diskusije-resenja-zadatka.component.scss']
})
export class DiskusijeResenjaZadatkaComponent implements OnInit {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);

  headerText: string;

  diskusije$: BehaviorSubject<Diskusija[] | null> = new BehaviorSubject<Diskusija[] | null>(null);

  constructor(private breakpointObserver: BreakpointObserver, private predmetService: PredmetService,
    private diskusijaService: DiskusijaService, private router: Router, private route: ActivatedRoute) {

    this.route.paramMap
      .pipe(switchMap((params) => {
        const id = Number(params.get('id'));
        console.log(id);

        // TODO: srediti ovaj kod tj. da ova komponenta ne bude samo za resenja namenjena

        if (!this.router.url.includes('resenja')) {
          this.headerText = "Diskusije za zadatak"
          return this.diskusijaService.getDiskusijeZadatka(id);
        }
        else {
          this.headerText = "Diskusije sa rešenjima"
          return this.diskusijaService.getDiskusijeSaResenjemZadatka(id);
        }

      })).subscribe(diskusije => {
        console.log(diskusije)
        this.diskusije$.next(diskusije);
      });
  }

  ngOnInit(): void {
  }

}

import { PredmetService } from './../../services/predmet.service';
import { map, Observable, shareReplay, BehaviorSubject } from 'rxjs';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { Predmet } from 'src/app/models/predmet.model';
import { UserBasic } from 'src/app/models/user-basic.model';
import { AuthService } from 'src/app/services/auth.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'predmet',
  templateUrl: './predmet.component.html',
  styleUrls: ['./predmet.component.scss']
})
export class PredmetComponent implements OnInit {
  user: UserBasic | null = null;


  navLink$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([
    {
      label: 'Pregled',
      link: './pregled',
      index: 0
    }, {
      label: 'Blanketi',
      link: './blanketi',
      index: 1
    }, {
      label: 'Zadaci',
      link: './zadaci',
      index: 2
    }, {
      label: 'Diskusije',
      link: './diskusije',
      index: 3
    }
  ]);

  activeLinkIndex = 0;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  predmet: Predmet;

  constructor(private breakpointObserver: BreakpointObserver,
    public authService: AuthService,
    private router: Router,
    public predmetService: PredmetService,
    private route: ActivatedRoute,
  ) {

    this.isHandset$.subscribe(is => {
      const zadaciLabel: string = is ? "Zadaci" : "Zadaci i pitanja";

      this.navLink$.next([
        {
          label: 'Pregled',
          link: './pregled',
          index: 0
        }, {
          label: 'Blanketi',
          link: './blanketi',
          index: 1
        }, {
          label: zadaciLabel,
          link: './zadaci',
          index: 2
        }, {
          label: 'Diskusije',
          link: './diskusije',
          index: 3
        }
      ]);
    })
  }

  ngOnInit(): void {
    this.router.events.subscribe((res) => {
      this.activeLinkIndex = this.navLink$.value.indexOf(this.navLink$.value.find((tab: any) => tab.link === '.' + this.router.url));
    });

    let id;
    this.route.paramMap.subscribe((params: ParamMap) => {
      id = +params.get('id')!

      this.predmetService.fetchPredmet(id);



      this.predmetService.getPredmet().subscribe({
        next: p => {
          if (p) {
            this.predmet = p!;
          }
        }
      });

      this.authService.getUserObserver()
        .subscribe((user) => {
          this.user = user;
        });
    });
  }

  ngOnDestroy() {
    this.predmetService.predmet$ = new BehaviorSubject<Predmet | null>(null);
  }

}

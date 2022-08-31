import { Title } from '@angular/platform-browser';
import { AuthService } from './../../services/auth.service';
import { apiError } from "../../types/apiError.type";
import { RezultatiPretrage } from "../../types/rezultatiPretrage.type";
import { PretragaService } from './../../services/pretraga.service';
import { debounceTime, Subject, Observable, BehaviorSubject, take, ReplaySubject, map, shareReplay } from 'rxjs';
import { Component, OnInit, ViewChild, ChangeDetectorRef, Directive, AfterContentChecked, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { User } from 'src/app/models/user.model';
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'pretraga',
  templateUrl: './pretraga.component.html',
  styleUrls: ['./pretraga.component.scss']
})
export class PretragaComponent implements OnInit {

  form: FormGroup;
  public subject: Subject<string> = new Subject();
  rezultati$: ReplaySubject<RezultatiPretrage | null> = new ReplaySubject<RezultatiPretrage | null>();
  user: User | null;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(
    private formBuilder: FormBuilder,
    public pretragaService: PretragaService,
    private cd: ChangeDetectorRef,
    private authService: AuthService,
    private breakpointObserver: BreakpointObserver,
    private title: Title
  ) {
    this.title.setTitle('Pretraga • e-Čitaonica');
  }

  @ViewChild('input') input: ElementRef; // ne radi ako je tip: MatInput

  ngAfterViewInit() {
    this.form.controls['query'].setValue(this.pretragaService.query);
    this.input.nativeElement.focus();

    this.cd.detectChanges();
  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      query: ['']
    });

    this.authService.getUserObserver().subscribe((user) => {
      this.user = user as User;
    });

    // inicijalizacija

    this.rezultati$.next(this.pretragaService.rezultati$.value);

    this.subject
      .pipe(debounceTime(300))
      .subscribe({
        next: (text: string) => {
          if (text === "") {
            this.pretragaService.query = "";
            this.pretragaService.rezultati$.next(null);
            return;
          }

          // ovako ne radi po drugi put:
          // this.pretragaService.pretrazi(text).subscribe(this.rezultati$);

          // TODO: kad se odrutira i rutira nazad na pretragu, ne osvezava se view
          // ovo podrzava i error handling:
          this.pretragaService.pretrazi(text).subscribe({
            next: res => {
              this.rezultati$.next(res);
              //console.log(text, this.rezultati$.value);
            },
            error: (err: apiError) => {
              console.log(err.error.msg);
            }
          });
        }
      });
  }

  onKeyUp(): void {
    this.subject.next('');
  }
}
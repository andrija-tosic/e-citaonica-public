import { Diskusija } from './../../models/diskusija.model';
import { PredmetService } from './../../services/predmet.service';
import { Predmet } from './../../models/predmet.model';
import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BehaviorSubject, switchMap } from 'rxjs';
import { AUTORIZACIJA_DISABLED, constants } from 'src/app/constants';
import { Komentar } from 'src/app/models/komentar.model';
import { UserBasic } from 'src/app/models/user-basic.model';
import { AuthService } from 'src/app/services/auth.service';
import { DiskusijaService } from 'src/app/services/diskusija.service';
import { ActiveCommentEnum } from 'src/app/types/activeCommentEnum.type';
import { ActiveCommentInterface } from 'src/app/types/activeCommentInterface.type';
import { apiError } from 'src/app/types/apiError.type';
import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';
import { PrijaviDialogComponent } from '../prijavi-dialog/prijavi-dialog.component';
import { Router } from '@angular/router';

@Component({
  selector: 'comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss']
})
export class CommentComponent implements OnInit, AfterViewInit {
  @ViewChild('commentRef') commentRef: ElementRef;
  @Input() diskusija: Diskusija;
  @Input() comment: Komentar | undefined;
  @Input() activeComment$: BehaviorSubject<ActiveCommentInterface | null>;
  @Input() subtreeCollapsed: boolean = false;
  @Input() scroll: string = "";
  @Input() disabled: boolean = false;
  @Input() showDiskusija: boolean = false;
  @Input() hideOptions: boolean = false;
  @Output('onDelete') onDelete: EventEmitter<Komentar> = new EventEmitter<Komentar>();
  user$: BehaviorSubject<UserBasic | null>;
  predmet$: BehaviorSubject<Predmet | null>;

  replying: boolean = false;
  activeCommentEnum = ActiveCommentEnum;

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  constructor(public authService: AuthService,
    private diskusijaService: DiskusijaService,
    private dialog: MatDialog,
    private _snackbar: MatSnackBar,
    private predmetService: PredmetService,
    private router: Router) {
    this.user$ = authService.getUserObserver();
    this.predmet$ = predmetService.getPredmet();
  }

  ngAfterViewInit(): void {
    if (this.scroll == this.comment?.id?.toString()) {
      this.commentRef.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
      setTimeout(_ => {
        this.commentRef.nativeElement.classList.toggle("focused");
        setTimeout(_ => this.commentRef.nativeElement.classList.toggle("focused"), 2000);
      }
        , 600);
    }
  }

  profesorPredmeta(): boolean {
    return this.user$.value?.tip === 'profesor'
      && this.predmet$?.value?.profesori?.map(p => p.id).includes(this.user$.value?.id) || false;
  }

  ngOnInit(): void {
    this.comment?.komentari?.sort((a, b) => new Date(b.datumKreiranja).getTime() - new Date(a.datumKreiranja).getTime());
  }


  isReplying(): boolean {
    if (!this.activeComment$?.value) {
      return false;
    }
    return (
      this.activeComment$.value.id === this.comment?.id &&
      this.activeComment$.value.type === this.activeCommentEnum.replying
    )
  }

  isEditing(): boolean {
    if (!this.activeComment$?.value) {
      return false;
    }
    return (
      this.activeComment$.value.id === this.comment?.id &&
      this.activeComment$.value.type === this.activeCommentEnum.editing
    );
  }

  onOdgovoriClick() {
    if (this.isReplying()) return this.activeComment$.next(null);
    this.activeComment$?.next({
      id: this.comment?.id || -1,
      type: this.activeCommentEnum.replying
    })
  }

  onEditClick() {
    if (this.isEditing()) return this.activeComment$.next(null);
    this.activeComment$?.next({
      id: this.comment?.id || -1,
      type: this.activeCommentEnum.editing
    })
  }

  onChildSubmit(komentar: Komentar) {
    this.diskusijaService.postKomentar({ objavaId: this.comment?.id, ...komentar })
      .subscribe({
        next: (komentar) => {
          if (this.comment && !this.comment.komentari)
            this.comment.komentari = [];
          this.comment?.komentari?.unshift(komentar);
          this.activeComment$.next(null);
        },
        error: (err: apiError) => console.error(err.error)
      });
  }

  collapse() {
    if (!this.comment?.komentari?.length) return;
    this.subtreeCollapsed = !this.subtreeCollapsed;
  }

  toggleZahvaljena() {
    if (!this.comment) return;
    this.comment.zahvaljena = !this.comment?.zahvaljena;
    this.comment?.zahvaljena ? this.comment.brZahvalnica++ : this.comment.brZahvalnica--;

    this.diskusijaService.zahvaliToggle(this.comment?.id || 0).subscribe({

    });
  }

  onEditSubmit(komentar: Komentar) {
    this.diskusijaService.updateKomentar(komentar).subscribe({
      next: (res: Komentar) => {
        if (this.comment) {
          this.comment.datumIzmene = res.datumIzmene;
          this.comment.sadrzaj = res.sadrzaj;
          this.comment.dodaci = res.dodaci;
          this.activeComment$.next(null);
        }
      },
      error: (err: apiError) => console.log(err.error)
    })
  }

  onTacnostToggle() {
    if (this.comment?.id)
      this.diskusijaService.tacnostToggle(this.comment?.id)
        .subscribe({
          next: (msg) => {
            if (this.comment) {
              if (this.comment.potvrdilacResenja) {
                this.comment.potvrdilacResenja = undefined;
                // this.comment.prihvacen = false;
              }
              else {
                this.comment.potvrdilacResenja = this.authService.user;
                // this.comment.prihvacen = true;
              }
            }
          },
          error: (msg) => console.log(msg)
        });
  }

  onDeleteClick() {
    this.onDelete.emit(this.comment);
  }

  onChildDelete(komentar: Komentar) {
    if (!komentar.id) return;
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, { data: { title: "Da li ste sigurni da želite da obrišete komentar?" } });
    dialogRef.afterClosed().subscribe(res => {
      if (!res || !komentar.id) return;
      this.diskusijaService.deleteKomentar(komentar.id)
        .subscribe({
          next: (msg) => {
            this._snackbar.open('Komentar obrisan.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
            if (this.comment?.komentari)
              this.comment.komentari = this.comment.komentari?.filter(k => k.id != komentar.id);
          },
          error: (msg) => {
            console.log(msg);
            this._snackbar.open('Došlo je do greške.', 'OK', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
          }
        })
    })
  }

  onPrijaviClick() {
    console.log("prijava");
    this.dialog.open(PrijaviDialogComponent, {
      maxWidth: '500px',
      width: '90%'
    }).afterClosed()
      .subscribe((res) => {
        if (!res || !this.comment?.id) return;
        this.diskusijaService.prijaviObjavu(this.comment.id, res.tekst)
          .subscribe({
            next: (res) => this._snackbar.open('Komentar prijavljen.', 'OK', { ...constants.snackbarPosition, duration: 3000 }),
            error: (err) => this._snackbar.open('Došlo je do greške, neuspešna prijava.', 'OK', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] })
          });
      })
  }

  onPrihvatiOdgovor() {
    if (this.comment?.id)
      this.diskusijaService.prihvatiToggle(this.comment?.id)
        .subscribe({
          next: (msg) => {
            if (this.comment) {
              this.comment.prihvacen = !this.comment.prihvacen;
            }
          },
          error: (msg) => console.log(msg)
        });
  }

  onDiskusijaClick() {
    console.log(this.comment);
    this.router.navigate([`/predmet/${this.comment?.diskusija?.predmet?.id}/diskusije/${this.comment?.diskusija?.id}`], {
      state: {
        scrollAnchor: `${this.comment?.id}`
      }
    });
  }
}
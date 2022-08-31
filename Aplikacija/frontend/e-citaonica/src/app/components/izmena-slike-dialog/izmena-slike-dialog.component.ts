import { Upload } from './../../services/service.helper';
import { apiError } from "../../types/apiError.type";
import { Observable, BehaviorSubject } from 'rxjs';
import { DataService } from 'src/app/services/data.service';
import { User } from './../../models/user.model';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
  selector: 'izmena-slike-dialog',
  templateUrl: './izmena-slike-dialog.component.html',
  styleUrls: ['./izmena-slike-dialog.component.scss']
})
export class IzmenaSlikeDialogComponent implements OnInit {

  slikaURL: string;
  imageFile: any;

  loader = this.loadingBar.useRef();
  uploading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(public dialogRef: MatDialogRef<IzmenaSlikeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public user: User,
    private dataService: DataService,
    private loadingBar: LoadingBarService) { }

  ngOnInit(): void {
    this.slikaURL = this.user.slikaURL;
  }

  onConfirm(): void {

    this.loader.start();
    this.uploading$.next(true);
    this.dialogRef.disableClose = true;

    if (this.slikaURL === '') {
      this.dataService.deleteUserImage(this.user).subscribe(msg => {
        this.loader.stop();
        this.uploading$.next(false);
        this.user.slikaURL = '';
        this.dialogRef.disableClose = false;
        this.dialogRef.close(this.user.slikaURL);
      });
    }
    else {

      this.dataService.changeUserImage(this.imageFile, this.user).subscribe({
        next: (report) => {
          this.loader.stop();
          this.uploading$.next(false);
          this.user.slikaURL = report.path;
          this.dialogRef.disableClose = false;
          this.dialogRef.close(this.user.slikaURL);
        },
        error: (err: apiError) => console.log(err)
      });
    }

  }

  onDismiss(): void {
    this.dialogRef.close(null);
  }

  onFileChanged(event: any) {
    this.imageFile = event.target.files[0];

    const reader = new FileReader();
    reader.readAsDataURL(this.imageFile);
    reader.onload = (e: any) => {
      this.slikaURL = e.target.result;
    };
  }

  obrisiSliku() {
    this.slikaURL = '';
  }

}

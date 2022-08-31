import { HttpEvent, HttpEventType } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ThrottlingUtils } from '@azure/msal-common';
import { Dodatak } from 'src/app/models/dodatak.model';
import { DataService } from 'src/app/services/data.service';
import { Upload } from 'src/app/services/service.helper';

interface UploadingDodatak {
  dodatak : Dodatak,
  progress : number
}

@Component({
  selector: 'upload-dodatak',
  templateUrl: './upload-dodatak.component.html',
  styleUrls: ['./upload-dodatak.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi:true,
      useExisting: UploadDodatakComponent
    }
  ]
})
export class UploadDodatakComponent implements ControlValueAccessor {
  @ViewChild('fileInput') fileInputRef: ElementRef;
  dodaci : Dodatak[] = [];
  @Input() disabled : boolean = false;

  onChange = (value : Dodatak[]) => {};
  onTouched = () => { };
  touched = false;

  public uploading: UploadingDodatak[] = [];

  constructor(private dataService : DataService) { }

  filesSelected(files : any) {
    if (!files) return;

    if (!this.touched) {
      this.touched = true;
      this.onTouched();
    } 

    files = Array.from(files);

    files.forEach((file: any) => {
      const upload : UploadingDodatak = {
        dodatak: {
          id: 0,
          naziv: file.name,
          sadrzaj: ''
        },
        progress: 0
      }
      this.uploading.push(upload);

      this.dataService.uploadFileWithProgress(file)
        .subscribe({
          next: ((report : Upload) => {
            if (report.state == 'DONE') {
              this.uploading = this.uploading.filter(u => u != upload);
              if (report.path) {
                upload.dodatak.sadrzaj = report.path;
                this.dodaci.push(upload.dodatak);
                this.onChange(this.dodaci);
              }
            }
            else {
              upload.progress = report.progress;
            }
          }),
          error: ((error) => {
            this.uploading = this.uploading.filter(u => u != upload);
          }) 
        })
    });
  }

  remove(dodatak : Dodatak) {
    this.dodaci = this.dodaci.filter(d => d.sadrzaj != dodatak.sadrzaj);
    this.onChange(this.dodaci);
  }

  writeValue(dodaci: Dodatak[]): void {
    this.dodaci = dodaci ? dodaci : [];
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled : boolean) {
    this.disabled = disabled;
  }
}

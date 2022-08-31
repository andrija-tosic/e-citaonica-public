import { Component, Inject, OnInit } from '@angular/core';
import { Form, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'prijavi-dialog',
  templateUrl: './prijavi-dialog.component.html',
  styleUrls: ['./prijavi-dialog.component.scss']
})
export class PrijaviDialogComponent implements OnInit {
  form : FormGroup;
  constructor(
    public dialogRef: MatDialogRef<PrijaviDialogComponent>,
    ) {
    this.form = new FormGroup({
      tekst : new FormControl("", [
        Validators.required, 
        Validators.maxLength(256)]
      )
    })
  }

  ngOnInit(): void {
  }

  onConfirm(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.value);
  }

  onDismiss(): void {
    // Close the dialog, return false
    this.dialogRef.close(false);
  }
}

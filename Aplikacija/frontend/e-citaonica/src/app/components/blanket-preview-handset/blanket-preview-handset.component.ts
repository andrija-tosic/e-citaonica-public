import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, Observable, shareReplay } from 'rxjs';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'blanket-preview-handset',
  templateUrl: './blanket-preview-handset.component.html',
  styleUrls: ['./blanket-preview-handset.component.scss']
})
export class BlanketPreviewHandsetComponent implements OnInit {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );



  constructor(public dialogRef: MatDialogRef<BlanketPreviewHandsetComponent>,
    @Inject(MAT_DIALOG_DATA) public blanket: any,
    private breakpointObserver: BreakpointObserver, private router: Router) {
    this.router.events.subscribe(e => {
      if (e instanceof NavigationStart) {
        this.dialogRef.close();
      }
    })
  }

  ngOnInit(): void {
  }

}

import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { SpinnerService } from 'src/app/services/spinner.service';

@Component({
  selector: 'spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss'],
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),

        animate('100ms 500ms', style({ opacity: 1 }))

      ]),
      transition(':leave', [
        animate('100ms', style({ opacity: 0 }))
      ])
    ])
  ]
})
export class SpinnerComponent implements OnInit {

  @Input('showSpinner') showSpinner = false;

  constructor(
    // private spinnerService: SpinnerService,
    // private cdRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    // this.spinnerService.getSpinnerObserver().subscribe({
    //   next: (status : boolean) => { 
    //     // console.log(status);
    //     this.showSpinner = status;
    //     this.cdRef.detectChanges();
    //   } 
    // })
  }

}

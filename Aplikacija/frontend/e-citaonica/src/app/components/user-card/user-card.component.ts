import { Component, Input, OnInit } from '@angular/core';
import { User } from 'src/app/models/user.model';

@Component({
  selector: 'user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss']
})
export class UserCardComponent implements OnInit {

  @Input() user: User;
  @Input() isVertical = false;
  collapsed = true;

  constructor() { }

  ngOnInit(): void {
  }

  mailTo(event: any) {
    window.location.href = `mailto:${this.user?.email}`;
    event.stopPropagation();
  }

  collapse(event: any) {
    this.collapsed = !this.collapsed;
    event.stopPropagation();
  }

}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Preporuka } from 'src/app/models/DTOs/preporuka.dto';

@Component({
  selector: 'preporuka-card',
  templateUrl: './preporuka-card.component.html',
  styleUrls: ['./preporuka-card.component.scss']
})
export class PreporukaCardComponent implements OnInit {
  @Input("student") student: Preporuka | undefined;
  @Input("added") added: boolean = false;
  @Output("onAdd") onAdd = new EventEmitter<Preporuka>();
  @Output("onRemove") onRemove = new EventEmitter<Preporuka>();
  constructor() { }

  ngOnInit(): void {
  }

  onAddClick() {
    this.onAdd.emit(this.student);
  }

  onRemoveClick() {
    this.onRemove.emit(this.student);
  }
}

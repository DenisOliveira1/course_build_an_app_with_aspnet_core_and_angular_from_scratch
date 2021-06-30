import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { MemberModel } from 'src/app/_models/memberModel';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  // Com essa linha ativa, o css desse arquivo vai se tornal global
  // encapsulation: ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit {

  // Anotação para receber propriedades para um componente pai
  @Input() member: MemberModel;
  
  constructor() { }

  ngOnInit(): void {
  }

}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() usersFromHomeComponent : any; // Usado para receber popriedades de um componente pai
  @Output() cancelRegisterToHomeComponent = new EventEmitter(); // Usado para enviar popriedades para um componente pai

  model: any = {};

  constructor() { }

  ngOnInit(): void {
  }

  register(){
    console.log(this.model);
  }

  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

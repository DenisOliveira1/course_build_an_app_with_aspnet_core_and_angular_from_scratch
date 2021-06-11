import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegisterToHomeComponent = new EventEmitter(); // Usado para enviar popriedades para um componente pai

  model: any = {};

  constructor(
    private accountService : AccountService
  ) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
    this.cancel();
  }


  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

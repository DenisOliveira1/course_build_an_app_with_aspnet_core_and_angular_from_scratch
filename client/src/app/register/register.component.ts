import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
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
    private accountService : AccountService,
    private toastr : ToastrService
  ) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.toastr.success("Registered successfully");
      this.cancel();
    }, error => {
      if (error){
        if(Array.isArray(error)){
          for (let line of error){
            this.toastr.error(line);
          }
        }
      }
    })
  }

  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

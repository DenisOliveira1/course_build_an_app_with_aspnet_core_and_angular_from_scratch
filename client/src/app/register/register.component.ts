import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // Anotação para enviar propriedades para um componente pai
  @Output() cancelRegisterToHomeComponent = new EventEmitter();

  model: any = {};

  constructor(
    private accountService : AccountService,
    private toastr : ToastrService
  ) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response => {
      // Só entra aqui se o retorno não for um erro
      console.log(response);
      this.toastr.success("Registered successfully");
      this.cancel();
    }, error => {
        // Só entra aqui se o retorno for um erro
        if(Array.isArray(error)){
          for (let line of error){
            this.toastr.error(line);
          }
        }
    })
  }

  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

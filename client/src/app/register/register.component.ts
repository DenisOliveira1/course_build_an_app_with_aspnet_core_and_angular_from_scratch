import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
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
  registerForm: FormGroup;

  constructor(
    private accountService : AccountService,
    private toastr : ToastrService,
    private fb : FormBuilder
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.registerForm = new FormGroup({
      // Os nomes devem ser iguais aos presentes no formControlName do HTML
      // Não precisa no NgModel mais
      username: new FormControl('', [Validators.required, Validators.minLength(4)]),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues("password")])
    });

    // Quando o campo password for alterado as validações do compo confirmPassword serão verificadas novamente
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })

  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control : AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true};
    }
  }

  register(){
    console.log(this.registerForm.value);

    // this.accountService.register(this.model).subscribe(response => {
    //   // Só entra aqui se o retorno não for um erro
    //   console.log(response);
    //   this.toastr.success("Registered successfully");
    //   this.cancel();
    // }, error => {
    //     // Só entra aqui se o retorno for um erro
    //     if(Array.isArray(error)){
    //       for (let line of error){
    //         this.toastr.error(line);
    //       }
    //     }
    // })
  }

  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

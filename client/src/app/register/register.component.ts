import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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

  registerForm: FormGroup;
  maxDate: Date;
  validationErrors : string[] = [];

  constructor(
    private accountService : AccountService,
    private toastr : ToastrService,
    private fb : FormBuilder,
    private router : Router
  ) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      username: ['',[Validators.required, Validators.minLength(2)]],
      knownAs: ['',Validators.required],
      dateOfBirth: ['',Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      gender: ['male'],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues("password")]]
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
    this.accountService.register(this.registerForm.value).subscribe(response => {
      // Só entra aqui se o retorno não for um erro
      this.toastr.success("Registered successfully");
      this.router.navigateByUrl("/memebrs");
    }, error => {
        // Só entra aqui se o retorno for um erro
        this.validationErrors = error;

        // if(Array.isArray(error)){
        //   for (let line of error){
        //     this.toastr.error(line);
        //   }
        // }
    })
  }

  cancel(){
    this.cancelRegisterToHomeComponent.emit(false);
  }

}

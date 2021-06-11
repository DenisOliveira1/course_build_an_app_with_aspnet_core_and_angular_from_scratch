import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UserModel } from '../_models/userModel';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  loggedIn: boolean = false;

  // Importar algo como private significa que só pode ser usado no .ts
  // Importar como public permite seu uso no .html também
  constructor(
    public accountService: AccountService
  ) { }

  ngOnInit(): void {

  }

  // Coloca o user no observable currentUser$ do service
  login(){
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  // Coloca null no observable currentUser$ do service
  logout(){
    this.accountService.logout();
  }  

}

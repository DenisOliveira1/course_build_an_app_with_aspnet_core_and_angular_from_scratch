import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { UserModel } from './_models/userModel';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title: string = 'The Datting App';
  users: any;

  constructor(
    private httpClient: HttpClient,
    private accountService: AccountService
  ) {

  }

  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  // Caso tenha um usuário no localStorage coloca ele no observable currentUser$ do service
  setCurrentUser(){
    const user: UserModel = JSON.parse(localStorage.getItem("user"));
    this.accountService.setCurrentUser(user);
  }

  // Pega a lista de usuários cadastrados no banco de dados para exibir
  getUsers(){
    this.httpClient.get("https://localhost:5001/api/users").subscribe(response => {
      this.users = response;
    }, error => {
      console.log(error);
    })
  }

}

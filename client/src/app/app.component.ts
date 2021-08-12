import { Component, OnInit } from '@angular/core';
import { UserModel } from './_models/userModel';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title: string = 'The Datting App';

  constructor(
    private accountService: AccountService,
    private presence: PresenceService
  ) {

  }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  // Caso tenha um usuário no localStorage coloca ele no observable currentUser$ do service
  setCurrentUser(){
    const user: UserModel = JSON.parse(localStorage.getItem("user"));
    if (user){
      this.accountService.setCurrentUser(user);
      this.presence.createHubConnection(user);
    }
  }

}

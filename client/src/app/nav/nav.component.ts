import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  loggedIn: boolean = false;

  constructor(
    private accountService: AccountService
  ) { }

  ngOnInit(): void {
    this.getCurrentUser();
  }

  // Define loggedIn como true e coloca ele no observable currentUser$
  login(){
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.loggedIn = true;
    }, error => {
      console.log(error);
    })
  }

  // Define loggedIn como false e define observable currentUser$ como null
  logout(){
    this.accountService.logout();
    this.loggedIn = false;
  }

 // Define loggedIn de acordo tenha um user no observable currentUser$
  getCurrentUser(){
    this.accountService.currentUser$.subscribe(user => {
      this.loggedIn = !!user;
    }, error => {
      console.log(error);
    })
  }
  

}

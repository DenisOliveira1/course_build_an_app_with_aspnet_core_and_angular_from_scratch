import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
    public accountService: AccountService,
    private router: Router,
    private toastr : ToastrService
  ) { }

  ngOnInit(): void {
  }

  // Coloca o user no observable currentUser$ do service
  login(){
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl("/members");
      this.toastr.success("Logged in successfully");
    },error => {
      if (error){
        if(Array.isArray(error)){
          for (let line of error){
            this.toastr.error(line);
          }
        }
      } 
    })
  }

  // Coloca null no observable currentUser$ do service
  logout(){
    this.accountService.logout();
    this.router.navigateByUrl("/");
    this.toastr.success("Logged out successfully");
  }  

}

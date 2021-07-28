import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { MemberModel } from 'src/app/_models/memberModel';
import { Pagination } from 'src/app/_models/params/pagination';
import { UserParams } from 'src/app/_models/params/userParams';
import { UserModel } from 'src/app/_models/userModel';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: MemberModel[];
  pagination : Pagination;
  userParams : UserParams; // Essa classe precisa do user logado também
  genderList = [{
    value: "male",
    displayValue: "Males"
  },{
    value: "female",
    displayValue: "Females"
  }]

  constructor(
    private memberService : MembersService
  ) { 
    // pega cache userParams salvo
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.userParams).subscribe(response =>{
      // update cache userParams
      this.memberService.setUserParams(this.userParams);
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event : any){
    this.userParams.pageNumber = event.page;
    // update cache userParams
    this.memberService.setUserParams(this.userParams);
    this.loadMembers();
  }

  resetFilters(){
    // update cache userParams
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

}

import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/_modals/roles-modal/roles-modal.component';
import { Pagination } from 'src/app/_models/params/pagination';
import { RolesParams } from 'src/app/_models/params/rolesParams';
import { UserModel } from 'src/app/_models/userModel';
import { AdminService } from 'src/app/_services/admin.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users: Partial<UserModel[]>;
  pagination: Pagination;
  rolesParams: RolesParams;
  bsModalRef: BsModalRef;

  constructor(
    private adminService: AdminService,
    private memberService: MembersService,
    private modalService: BsModalService
  ) { 
    this.rolesParams = this.memberService.getRolesParams();
  }

  ngOnInit(): void {
    this.getUserWithRoles();
  }

  getUserWithRoles(){
    this.adminService.getUsersWithRoles(this.rolesParams).subscribe(response => {
      // update cache rolesParams
      this.memberService.setRolesParams(this.rolesParams);
      this.users = response.result;
      this.pagination = response.pagination;
      // console.log(response);
      // console.log(this.pagination);
    })
  }

  pageChanged(event : any){
    this.rolesParams.pageNumber = event.page;
    // update cache rolesParams
    this.memberService.setRolesParams(this.rolesParams);
    this.getUserWithRoles();
  }

  resetFilters(){
    // update cache rolesParams
    this.rolesParams = this.memberService.resetRolesParams();
    this.getUserWithRoles();
  }

  openRolesModal(){

    // Maneira 1 de passar parametros ao modal
    const initialState = {
      list: [
        'Open a modal with component',
        'Pass your data',
        'Do something else',
        '...'
      ],
      title: 'Modal with component'
    };

    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.closeBtnName = 'Close'; // Maneira 2

  }

}

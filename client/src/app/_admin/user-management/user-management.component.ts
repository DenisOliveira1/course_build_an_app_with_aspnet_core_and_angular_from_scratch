import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
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
    private modalService: BsModalService,
    private toastr: ToastrService
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

  openRolesModal(user: UserModel){

    // Maneira 1 de passar parametros ao modal
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    };

    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    // Maneira 2
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => {
        const rolesToUpdate =  {
          roles: [...values.filter(el => el.checked === true).map(el => el.name)]
        }
        if (rolesToUpdate){
          this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
            user.roles = [...rolesToUpdate.roles];
            this.toastr.success("Roles updated successfully");
          })
        }
    });
    
  }

  private getRolesArray(user){
    const roles = [];
    const userRoles = user.roles;
    const avaliableRoles: any[] = [
      {name: 'Admin', value: "admin"},
      {name: 'Moderator', value: "moderator"},
      {name: 'Member', value: "member"},
    ]

    avaliableRoles.forEach(role => {
      let isMatch = false;
      // Se o user tiver a role marca como checked
      for (const userRole of userRoles){
        if (role.name === userRole){
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      // Se o user não tiver a role não marca como checked
      if (!isMatch){
        role.checked = false;
        roles.push(role);
      }
    })

    return roles;
  }

}

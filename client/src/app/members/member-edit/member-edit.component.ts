import { FunctionCall } from '@angular/compiler';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { MemberModel } from 'src/app/_models/memberModel';
import { UserModel } from 'src/app/_models/userModel';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})

export class MemberEditComponent implements OnInit {

  // Anotação para referenciar o formulário dentro do componente
  @ViewChild("editForm") editForm: NgForm;
  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event : any){
    if (this.editForm.dirty){
      $event.returnValue = true;
    }
  }

  member : MemberModel;
  user : UserModel;

  constructor(
    private accountService : AccountService,
    private memberService : MembersService,
    private toastr: ToastrService,
    private router: Router
  ) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(
      user => this.user = user
    )
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.memberService.getMember(this.user.username).subscribe(member => {
      this.member = member;
    })
  }

  updateMember(){
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastr.success("Profile updated successfully");
      // Mesmo apagando as alterações feitas em um form o status dirty FunctionCall
      // Após salvar o form o status do form é redefinido e o dirty some, desativando o botão e o alerta novamente
      this.editForm.reset(this.member);
    })
  }

  deleteAccount(){
    this.memberService.deleteUser().subscribe(() => {
      this.router.navigateByUrl("/");
      this.toastr.warning("Account deleted successfully");
      this.accountService.setCurrentUser(null);
    })
  }
}

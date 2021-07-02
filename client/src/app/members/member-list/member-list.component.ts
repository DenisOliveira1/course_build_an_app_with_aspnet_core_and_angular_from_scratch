import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MemberModel } from 'src/app/_models/memberModel';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members$: Observable<MemberModel[]>;

  constructor(
    private memberService : MembersService
  ) { }

  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }

}

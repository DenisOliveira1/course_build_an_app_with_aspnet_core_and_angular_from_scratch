import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Tools } from 'src/app/_helpers/Tools';
import { MemberModel } from 'src/app/_models/memberModel';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  // Com essa linha ativa, o css desse arquivo vai se tornal global
  // encapsulation: ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit {

  // Anotação para receber propriedades para um componente pai
  @Input() member: MemberModel;
  
  constructor(
    private memberService : MembersService,
    private toastr : ToastrService,
    public presenceService: PresenceService // Public pois aqui vamos acessar um observable desse service
  ) { }

  ngOnInit(): void {
  }

  addLike(member: MemberModel){
    this.memberService.addLike(member.username).subscribe(() => {
      this.toastr.success("You have liked " + Tools.titleCaseText(member.username))
    })  
  }

}

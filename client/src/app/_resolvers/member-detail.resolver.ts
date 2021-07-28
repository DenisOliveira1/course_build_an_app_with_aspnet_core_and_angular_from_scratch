import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { MemberModel } from '../_models/memberModel';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})

export class MemberDetailResolver implements Resolve<MemberModel> {

  constructor(
    private memberService: MembersService
  ) {
    
  }
  
  resolve(route: ActivatedRouteSnapshot): Observable<MemberModel> {
    // Caso fosse trabalhar com mais de um dado seria possivel retornar um objeto {}
    return this.memberService.getMember(route.paramMap.get("username"));
  }
  
}

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ObserveOnOperator } from 'rxjs/internal/operators/observeOn';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MemberModel } from '../_models/memberModel';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: "Bearer " + JSON.parse(localStorage.getItem("user")).token
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  // Já que os services são com singletons eles são um bom meio de armazenar dados
  members: MemberModel[];

  constructor(
    private httpClient : HttpClient
  ) { 
  }

  getMembers(){
    // Para não ficar indo no servidor toda hora uma copia dos dados ficara salva e será consultada enquanto estiver presente na memória
    // of transforma o dado em um observer, essa função precisa retornar um observer
    if (this.members?.length > 0) return of(this.members);
    
    return this.httpClient.get<MemberModel[]>(this.baseUrl + "users").pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(username : string){
    const member = this.members?.find(x => x.username === username);
    if (member !== undefined) return of(member);
    return this.httpClient.get<MemberModel>(this.baseUrl + "users/" + username);
    
  }

  updateMember(member : MemberModel){
    return this.httpClient.put(this.baseUrl + "users/", member).pipe(
      map(() => {
        // É necessário atualizar o array de membros alocados na memória
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
}

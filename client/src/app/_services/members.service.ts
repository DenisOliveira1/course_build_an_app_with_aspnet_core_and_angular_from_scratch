import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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

  constructor(
    private httpClient : HttpClient
  ) { 
  }

  getMembers(){
    return this.httpClient.get<MemberModel[]>(this.baseUrl + "users");
  }

  getMember(username : string){
    return this.httpClient.get<MemberModel>(this.baseUrl + "users/" + username);
  }
}

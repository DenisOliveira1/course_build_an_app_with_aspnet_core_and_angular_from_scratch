import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { UserModel } from '../_models/userModel';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<UserModel>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(
    private httpClient: HttpClient
    ) { }
  
  // Essa função retorna um observable. O pipe permiter manipular esse retorno ainda no service
  // Define observable currentUser$ como o user do retorno
  login(model: any){
    return this.httpClient.post(this.baseUrl + "account/login", model).pipe(
      map((response: UserModel) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  register(model: any){
    return this.httpClient.post(this.baseUrl + "account/register", model).pipe(
      map((response: UserModel) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }

  // Define observable currentUser$ como o user informado
  setCurrentUser(user: UserModel){

    user.roles = [];

    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem("user",JSON.stringify(user)); // Transforma em JSON
    this.currentUserSource.next(user);
  }

  // Define observable currentUser$ como null 
  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string){
    return JSON.parse(atob(token.split(".")[1]));
  }

}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserModel } from '../_models/userModel';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = "https://localhost:5001/api/"
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
          localStorage.setItem("user",JSON.stringify(user)); // Transforma em JSON
          this.currentUserSource.next(user);
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
          localStorage.setItem("user",JSON.stringify(user)); // Transforma em JSON
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }

  // Define observable currentUser$ como o user informado
  setCurrentUser(user: UserModel){
    this.currentUserSource.next(user);
  }

  // Define observable currentUser$ como null 
  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

}

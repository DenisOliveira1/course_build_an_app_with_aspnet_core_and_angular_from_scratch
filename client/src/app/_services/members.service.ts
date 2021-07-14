import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ObserveOnOperator } from 'rxjs/internal/operators/observeOn';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MemberModel } from '../_models/memberModel';
import { PaginatedResult } from '../_models/pagination';
import { UserModel } from '../_models/userModel';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  // Já que os services são com singletons eles são um bom meio de armazenar dados
  memberCache = new Map();
  userParams : UserParams; // Essa classe precisa do user logado também
  user : UserModel;

  //Ao invejatar outro service em um service não podemos fazer ao contrário também, isso causaria uma referencia circular
  constructor(
    private httpClient : HttpClient,
    private accountService: AccountService
  ) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params : UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }


  // A função recebe um objeto model com parametros e gera o outro objeto httpParams para enviar junto a requisição
  getMembers(userParams : UserParams){
    // console.log(Object.values(userParams).join("-"))
    var response = this.memberCache.get(Object.values(userParams).join("-"));
    if (response) return of (response);

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize)
    
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    // Um objeto params no método get, vai no endereço na rota ?minAge=x, em métodos como post e put, vai no body
    // Com {observe: "response", params} você tem acesso ao body e o header
    // Com {params}  você tem acesso ao body direto
    return this.getPaginatedResults<MemberModel[]>(this.baseUrl + "users", params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join("-"), response);
        return response
      })
    )
  }

  getMember(username : string){
    // console.log(this.memberCache);
    // Um membro pode estar varias vezesno Array, o find acessará o primeiro
    const cacheResults = [...this.memberCache.values()];
    const cacheMembers = cacheResults.reduce((arr, elem) => arr.concat(elem.result), []);
    const member = cacheMembers.find((x : MemberModel) => x.username === username) ;
    if (member) return of (member);

    return this.httpClient.get<MemberModel>(this.baseUrl + "users/" + username);
  }

  updateMember(member : MemberModel){
    return this.httpClient.put(this.baseUrl + "users/", member).pipe(
      map(() => {
      })
    );
  }

  setMainPhoto(photoId : number){
    return this.httpClient.put(this.baseUrl + "users/set-main-photo/" + photoId, {});
  }

  deletePhoto(photoId : number){
    return this.httpClient.delete(this.baseUrl + "users/delete-photo/" + photoId, {});
  }

  deleteUser(){
    return this.httpClient.delete(this.baseUrl + "users/delete-user/", {});
  }

  private getPaginationHeaders(pageNumber: number, pageSize : number) { 

    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }

  private getPaginatedResults<T>(url: string, params: HttpParams) {

    const paginatedResult : PaginatedResult<T> = new PaginatedResult<T>();

    return this.httpClient.get<T>(url, { observe: "response", params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get("Pagination")) {
          paginatedResult.pagination = JSON.parse(response.headers.get("Pagination"));
        }
        return paginatedResult;
      })
    );
  }

}

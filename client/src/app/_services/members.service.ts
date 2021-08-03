import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ObserveOnOperator } from 'rxjs/internal/operators/observeOn';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LikeParams } from '../_models/params/likeParams';
import { MemberModel } from '../_models/memberModel';
import { PaginatedResult } from '../_models/params/pagination';
import { UserModel } from '../_models/userModel';
import { UserParams } from '../_models/params/userParams';
import { AccountService } from './account.service';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';
import { MessageService } from './message.service';
import { MessageParams } from '../_models/params/messageParams';
import { RolesParams } from '../_models/params/rolesParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  // Já que os services são com singletons eles são um bom meio de armazenar dados
  memberCache = new Map();
  likeParams : LikeParams;
  userParams : UserParams; // Essa classe precisa do user logado também
  rolesParams : RolesParams;
  user : UserModel;

  //Ao invejatar outro service em um service não podemos fazer ao contrário também, isso causaria uma referencia circular
  constructor(
    private httpClient : HttpClient,
    private accountService: AccountService,
    private messageService: MessageService,
  ) { 
    this.updateCurrentUser();
  }

  // A função recebe um objeto model com parametros e gera o outro objeto httpParams para enviar junto a requisição
  getMembers(userParams : UserParams){
    // console.log(Object.values(userParams).join("-"))
    var response = this.memberCache.get(Object.values(userParams).join("-"));
    if (response) return of (response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize)

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    // Um objeto params do tipo HttpParams no método get, vai no endereço na rota ?minAge=x
    // Em métodos como post e put não é um objeto do tipo HttpParams e seu conteúdo vai no body
    // Com {observe: "response", params} você tem acesso ao body e o header
    // Com {params}  você tem acesso ao body direto
    return getPaginatedResults<MemberModel[]>(this.baseUrl + "users", params, this.httpClient).pipe(
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

 updateCurrentUser(){
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
      this.likeParams = new LikeParams();
      this.rolesParams = new RolesParams();
      this.messageService.messageParams = new MessageParams();
    })
  }
  
  getLikes(likeParams : LikeParams){

    let params = getPaginationHeaders(likeParams.pageNumber, likeParams.pageSize);

    params = params.append('predicate', likeParams.predicate);

    return getPaginatedResults<Partial<MemberModel[]>>(this.baseUrl + "likes", params, this.httpClient);
  }

  addLike(username: string){
    return this.httpClient.post(this.baseUrl + "likes/" + username, {});
  }

  // User  Params
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

  // Like Params
  getLikeParams(){
    return this.likeParams;
  }

  setLikeParams(params : LikeParams){
    this.likeParams = params;
  }

  resetLikeParams(){
    this.likeParams = new LikeParams();
    return this.likeParams;
  }

  // Roles Params
  getRolesParams(){
    return this.rolesParams;
  }

  setRolesParams(params : RolesParams){
    this.rolesParams = params;
  }

  resetRolesParams(){
    this.rolesParams = new RolesParams();
    return this.rolesParams;
  }

}

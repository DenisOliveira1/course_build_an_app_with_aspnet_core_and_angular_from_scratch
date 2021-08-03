import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { RolesParams } from '../_models/params/rolesParams';
import { UserModel } from '../_models/userModel';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  adminCache = new Map();

  constructor(
    private httpClient: HttpClient
  ) { }

  getUsersWithRoles(rolesParams: RolesParams){

    var response = this.adminCache.get(Object.values(rolesParams).join("-"));
    if (response) return of (response);

    let params = getPaginationHeaders(rolesParams.pageNumber, rolesParams.pageSize)

    return getPaginatedResults<Partial<UserModel[]>>(this.baseUrl + "admin/users-with-roles", params, this.httpClient).pipe(
      map(response => {
        this.adminCache.set(Object.values(rolesParams).join("-"), response);
        return response;
      })
    )
  }

  updateUserRoles(username: string, roles: string[]){
    // No post nãode passa o objeto HttpParams como no get, enmtão deve-se adicionar os parametros da query manualmente na url
    return this.httpClient.post(this.baseUrl + "admin/edit-roles/" + username + "?roles=" + roles, {})
  }
  
}

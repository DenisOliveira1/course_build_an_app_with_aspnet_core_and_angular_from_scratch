import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { UserModel } from '../_models/userModel';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(
    private accountService : AccountService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    let currentUser : UserModel;

    this.accountService.currentUser$.pipe(take(1)).subscribe(
      response => {
        currentUser = response;
      })

    if (currentUser){
      request = request.clone({
        setHeaders:{
          Authorization: "Bearer " + currentUser.token
        }
      })
    }

    return next.handle(request);
  }
}

import { stringify } from '@angular/compiler/src/util';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { UserModel } from '../_models/userModel';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(
    private toastr: ToastrService,
    private router: Router
  ) { }P

  // Esse método é chamado quando o cada usuário loga e conecta ele aos métodos UserIsOnline, UserIsOffline e GetOnlineUsers
  createHubConnection(user : UserModel){
    this.hubConnection = new HubConnectionBuilder()  
      .withUrl(this.hubUrl + "presence", {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.log(error));

      this.hubConnection.on("UserIsOnline", username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
            this.onlineUserSource.next([...usernames, username]);
        })
      })

      this.hubConnection.on("UserIsOffline", username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
          this.onlineUserSource.next([...usernames.filter(x => x != username)]);
      })
      })

      this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
        this.onlineUserSource.next(usernames);
      })

      this.hubConnection.on("NewMessageReceived", ({username, knownAs, message, photoUrl}) => {
        console.log(photoUrl);
        // this.toastr.info(knownAs + "<br><br> <img class='rounded-circle mr-2' src='./assets/user.png'>")
        this.toastr.info(knownAs + "<br>" + message)
          .onTap
          .pipe(take(1))
          .subscribe(() => this.router.navigateByUrl("/members/" + username + "?tab=3"))
      })
  }

  stopHubConnection(){
    if (this.hubConnection)
      this.hubConnection
        .stop()
        .catch(error => console.log(error));
  }
}

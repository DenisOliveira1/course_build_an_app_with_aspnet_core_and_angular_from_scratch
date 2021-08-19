import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { GroupModel } from '../_models/groupModel';
import { MessageModel } from '../_models/messageModel';
import { MessageParams } from '../_models/params/messageParams';
import { UserModel } from '../_models/userModel';
import { BusyService } from './busy.service';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  messageParams : MessageParams;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<MessageModel[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(
    private httpClient : HttpClient,
    private busyService : BusyService
  ) { }

  getMessages(messageParams : MessageParams){

    let params = getPaginationHeaders(messageParams.pageNumber, messageParams.pageSize)

    params = params.append('container', messageParams.container);

    return getPaginatedResults<MessageModel[]>(this.baseUrl + "messages", params, this.httpClient);
  }

  getMessageThread(username : string){
    return this.httpClient.get<MessageModel[]>(this.baseUrl + "messages/thread/" + username);
  }

  async sendMessage(username: string, content: string){
    // SendMessage é o nome do método sendo invocado no hub do backend
    return this.hubConnection.invoke("SendMessage", 
    {
      recipientUsername: username,
      content
    })
    .catch(error => console.log(error));
  }

  deleteMessage(id: number){
    return this.httpClient.delete(this.baseUrl + "messages/" + id);
  }

  // Message Params
  getMessageParams(){
    return this.messageParams;
  }

  setMessageParams(params : MessageParams){
    this.messageParams = params;
  }

  createHubConnection(user : UserModel, otherUsername: string){
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()  
      .withUrl(this.hubUrl + "message?user=" + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.log(error))
      .finally(() => this.busyService.idle());

      this.hubConnection.on("ReceiveMessageThread", messages => {
        this.messageThreadSource.next(messages);
      })

      this.hubConnection.on("NewMessage", message => {
        this.messageThreadSource.pipe(take(1)).subscribe(messages => {
          //BehaviorSubject não admite alterações, logo o operador [...] vai gerar um novo array ao invés de adicionar conteúdo
          //O novo array vai ser uma copia do anterior com a adição da nova mensagem
          this.messageThreadSource.next([...messages, message]);
        })
      })

      this.hubConnection.on("UpdatedGroup", (group: GroupModel) => {
        if(group.connections.some(x => x.username === otherUsername)){
            this.messageThread$.pipe(take(1)).subscribe(messages => {
              messages.forEach(message => {
                if (!message.dateRead){
                  message.dateRead = new Date(Date.now());
                }
              })
              this.messageThreadSource.next([...messages]);
            })
        }
      })
  }

  stopHubConnection(){
    if (this.hubConnection)
      this.messageThreadSource.next([]);
      this.hubConnection
        .stop()
        .catch(error => console.log(error));
  }

  private componentMethodCallSource = new Subject<any>();
  componentMethodCalled$ = this.componentMethodCallSource.asObservable();

}

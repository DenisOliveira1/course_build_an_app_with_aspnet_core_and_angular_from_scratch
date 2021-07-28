import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MessageModel } from '../_models/messageModel';
import { MessageParams } from '../_models/params/messageParams';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  messageParams : MessageParams;

  constructor(
    private httpClient : HttpClient
  ) { }

  getMessages(messageParams : MessageParams){

    let params = getPaginationHeaders(messageParams.pageNumber, messageParams.pageSize)

    params = params.append('container', messageParams.container);

    return getPaginatedResults<MessageModel[]>(this.baseUrl + "messages", params, this.httpClient);
  }

  getMessageThread(username : string){
    return this.httpClient.get<MessageModel[]>(this.baseUrl + "messages/thread/" + username);
  }

  sendMessage(username: string, content: string){
    return this.httpClient.post<MessageModel>(this.baseUrl + "messages", 
    {
      recipientUsername: username,
      content
    });
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

}

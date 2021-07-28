import { Component, OnInit } from '@angular/core';
import { MessageModel } from '../_models/messageModel';
import { Pagination } from '../_models/params/pagination';
import { MessageParams } from '../_models/params/messageParams';
import { MessageService } from '../_services/message.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages : MessageModel[] = [];
  pagination: Pagination;
  messageParams : MessageParams;
  loading = false;

  constructor(
    private messageService : MessageService,
    private toastr : ToastrService
  ) {
    // pega cache messageParams salvo
    this.messageParams = this.messageService.getMessageParams();
   }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.messageParams).subscribe(response => {
      // update cache messageParams
      this.messageService.setMessageParams(this.messageParams);
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading = false;
    })
  }  
  pageChanged(event : any){
    this.messageParams.pageNumber = event.page;
    // update cache messageParams
    this.messageService.setMessageParams(this.messageParams);
    this.loadMessages();
  }
  
  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
      this.toastr.success("The message has been delete successfully");
    })
  }

}

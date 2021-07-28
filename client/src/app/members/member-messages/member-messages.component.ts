import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Toast, ToastrService } from 'ngx-toastr';
import { MessageModel } from 'src/app/_models/messageModel';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @ViewChild("messageForm") messageForm : NgForm;
  @Input() messages : MessageModel[];
  @Input() username : string;

  messageContent: string;

  constructor(
    private messageService : MessageService,
    private toastr : ToastrService
  ) { }

  ngOnInit(): void {

  }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe( message => {
      this.messages.push(message);
      this.messageForm.reset();
      this.toastr.success("Message sent successfully");
    })
  }

}

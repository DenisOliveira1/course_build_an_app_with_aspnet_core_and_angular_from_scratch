import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Toast, ToastrService } from 'ngx-toastr';
import { MessageModel } from 'src/app/_models/messageModel';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @ViewChild("messageForm") messageForm : NgForm;
  @Input() username : string;
  @Input() activate : boolean;

  messageContent: string;
  loading = false;

  constructor(
    public messageService : MessageService, // Public pois aqui vamos acessar um observable desse service
    private toastr : ToastrService
  ) {

   }

  ngOnInit(): void {

  }

  sendMessage(){
    this.loading = true;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
      this.toastr.success("Message sent successfully");
    })
    .finally(() => this.loading = false);
  }

}

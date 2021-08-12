import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
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

  @ViewChild('scroll') private scroll: ElementRef;
  @ViewChild("messageForm") messageForm : NgForm;
  @Input() username : string;
  @Input() activate : boolean;

  messageContent: string;

  constructor(
    public messageService : MessageService, // Public pois aqui vamos acessar um observable desse service
    private toastr : ToastrService
  ) {
    this.scrollToBottom(); 
   }

  ngOnInit(): void {
    //Manda essa função para que o service consiga chamar ela de lá
    this.messageService.SendFunctionToService(this.scrollToBottom.bind(this));
  }

  ngAfterViewInit(){
    if (this.activate){
      this.scrollToBottom();
      this.activate = false;
    }
  }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
      this.toastr.success("Message sent successfully");
      this.scrollToBottom();
    })
  }

 async scrollToBottom(): Promise<void> {
  //Tempo para que o refresh seja feito no HTML e o scroll va até o final,
  // incluindo a nova menssagem que acabou de ser enviada
  await new Promise(r => setTimeout(r, 100));

    try {
        this.scroll.nativeElement.scrollTop = this.scroll.nativeElement.scrollHeight;
    } catch(err) {
      // console.log(err);
     }                 
  }

}

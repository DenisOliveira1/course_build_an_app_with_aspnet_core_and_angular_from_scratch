import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Tools } from 'src/app/_helpers/Tools';
import { MemberModel } from 'src/app/_models/memberModel';
import { MessageModel } from 'src/app/_models/messageModel';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  @ViewChild("memberTabs", {static: true}) memberTabs: TabsetComponent;

  member : MemberModel;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab : TabDirective;
  messages : MessageModel[] = []; // Se não inicializar o array na declaração tem que usar o optional channing operator ? no template html
  
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {

    // Acessa o resolver
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    // Por causa do ngIf do html é preciso carregar primeiro o membro, para assim criar os componentes e encontrar a tab
    // Porém mesmo colocando esse trecho dentro de loadMember() não deu certo
    // Então foi feito um resolver para recuperar o membro e o método loadMember foi deletado, pois o resolver já se encarrega disso
    // Foi preciso adicionar {static: true} ao referencia a tab tambem
    // Com o resolver, tendo certeza que o member sempre será carregado antes de iniciar o componente não precisa mais do ngIf no template
    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false

      }
    ]

    this.galleryImages = this.getImages();

  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }
    return imageUrls;
  } 

  addLike(){
    this.memberService.addLike(this.member.username).subscribe(() => {
      this.toastr.success("You have liked " + Tools.titleCaseText(this.member.username))
    })  
  }

  loadMessages(){
    this.messageService.getMessageThread(this.member.username).subscribe( response => {
      this.messages = response;
    })
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if ( this.activeTab.heading === "Messages" && this.messages.length === 0){
      this.loadMessages();
    }
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }

}

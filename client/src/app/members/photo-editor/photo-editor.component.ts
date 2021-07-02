import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { MemberModel } from 'src/app/_models/memberModel';
import { UserModel } from 'src/app/_models/userModel';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';
import { MemberEditComponent } from '../member-edit/member-edit.component';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})

export class PhotoEditorComponent implements OnInit {

  @Input() member : MemberModel;
  uploader : FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl + "users/add-photo";
  user: UserModel;

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr : ToastrService
    ) { 
      this.accountService.currentUser$.pipe(take(1)).subscribe(
        user => this.user = user
      )
    }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e : any){
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl,
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10*1024*1024 // 10mb
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem =(item, response, status, headers) => {
      if (response){
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
        this.toastr.success("The photos have been uploaded");
        
      }
    }
  }

  setMainPhoto(photo){
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain = false
        if (p.id === photo.id) p.isMain= true
      })

      this.toastr.success("Main photo has been updated");
    })
  }

  deletePhoto(photo){
    this.memberService.deletePhoto(photo.id).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id !== photo.id)

      this.toastr.success("The photo has been deleted");
    })
  }

}

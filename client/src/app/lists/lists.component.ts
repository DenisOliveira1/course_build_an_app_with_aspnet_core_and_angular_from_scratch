import { Component, OnInit } from '@angular/core';
import { LikeParams } from '../_models/likeParams';
import { MemberModel } from '../_models/memberModel';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  // Partial torna todas as propriedades do objeto opcionais
  members : Partial<MemberModel[]>;
  predicate = "liked";
  likeParams : LikeParams;
  pagination : Pagination;

  constructor(
    private memberService : MembersService
  ) {
    this.likeParams = this.memberService.getLikeParams();
   }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.likeParams).subscribe(response => {
          // update cache
          this.memberService.setLikeParams(this.likeParams);
          this.members = response.result;
          this.pagination = response.pagination;
    })
  }

  pageChanged(event : any){
    this.likeParams.pageNumber = event.page;
    // update cache
    this.memberService.setLikeParams(this.likeParams);
    this.loadLikes();
  }

  resetFilters(){
    // update cache
    this.likeParams = this.memberService.resetLikeParams();
    this.loadLikes();
  }

}

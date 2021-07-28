import { Component, OnInit } from '@angular/core';
import { LikeParams } from '../_models/params/likeParams';
import { MemberModel } from '../_models/memberModel';
import { Pagination } from '../_models/params/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-matches',
  templateUrl: './matches.component.html',
  styleUrls: ['./matches.component.css']
})
export class MatchesComponent implements OnInit {

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

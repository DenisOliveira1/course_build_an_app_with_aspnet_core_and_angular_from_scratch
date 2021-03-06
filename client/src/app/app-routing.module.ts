import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './_errors/not-found/not-found.component';
import { ServerErrorComponent } from './_errors/server-error/server-error.component';
import { TestErrorsComponent } from './_errors/test-errors/test-errors.component';
import { HomeComponent } from './home/home.component';
import { MatchesComponent } from './matches/matches.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { AdminPanelComponent } from './_admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';

const routes: Routes = [
  {path: "", component: HomeComponent},
  {
    path: "",
    runGuardsAndResolvers : "always",
    canActivate: [AuthGuard],
    children:[
      {path: "members", component: MemberListComponent},
      {path: "members/:username", component: MemberDetailComponent, resolve: {member: MemberDetailResolver}},
      {path: "member/edit", component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
      {path: "lists", component: MatchesComponent},
      {path: "messages", component: MessagesComponent},
      {path: "admin", component: AdminPanelComponent, canActivate: [AdminGuard]},
    ]
  },
  {path: "errors", component: TestErrorsComponent},
  {path: "not-found", component: NotFoundComponent},
  {path: "server-error", component: ServerErrorComponent},
  {path: "**", component: HomeComponent, pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

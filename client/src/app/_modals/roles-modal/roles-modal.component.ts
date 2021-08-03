import { Component, Input, OnInit, EventEmitter} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UserModel } from 'src/app/_models/userModel';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {

  // Variáveis que vem do initialState devem ser declaradas aqui
  // title: string;
  // list: any[] = [];
  // closeBtnName: string;

  @Input() updateSelectedRoles = new EventEmitter();
  user: UserModel;
  roles: any[];

  constructor(
    public bsModalRef: BsModalRef
  ) { }

  ngOnInit(): void {
  }

  updateRoles(){
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}

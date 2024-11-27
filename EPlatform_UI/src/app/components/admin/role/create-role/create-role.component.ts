import { Component, EventEmitter, inject, Output } from '@angular/core';
import { CreateRoleModel } from '../../models/roles/create-role-model';
import { DOCUMENT } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-role',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-role.component.html',
  styleUrl: './create-role.component.scss'
})
export class CreateRoleComponent {
  createRoleModel: CreateRoleModel = {
    roleName: ""
  }
  document = inject(DOCUMENT);
  @Output() create = new EventEmitter<CreateRoleModel>();

  showCreateForm(){
    var form = this.document.getElementById("create-form");
    if (form){
      form.style.display = 'block';
    }
  }

  hideCreateForm(){
    var form = this.document.getElementById("create-form");
    if (form){
      form.style.display = 'none';
    }
  }

  createRole(){
    this.create.emit(this.createRoleModel);
    this.hideCreateForm();
    this.createRoleModel.roleName = "";
  }

}

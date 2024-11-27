import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UpdateRoleModel } from '../../models/roles/update-role-model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-update-role',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './update-role.component.html',
  styleUrl: './update-role.component.scss'
})
export class UpdateRoleComponent {
  document = inject(DOCUMENT);
  @Output() update = new EventEmitter<UpdateRoleModel>();
  updateRoleModel: UpdateRoleModel = {
    name: "",
    roleId: ""
  }

  showUpdateRole(roleId: string, roleName: string){
    this.updateRoleModel = {
      name: roleName,
      roleId: roleId
    };
    var updateRoleElement = this.document.getElementById("update-form");
    if (updateRoleElement){
      updateRoleElement.style.display = 'block';
    }
  }

  updateRole(){

    this.update.emit(this.updateRoleModel);
    this.hideUpdateForm();
  }

  hideUpdateForm(){
    var updateRoleElement = this.document.getElementById("update-form");
    if (updateRoleElement){
      updateRoleElement.style.display = 'none';
    }
  }
}

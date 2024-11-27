import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AddNewRoleClaimModel } from '../../models/roles/add-role-claim-model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-add-new-claim',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-new-claim.component.html',
  styleUrl: './add-new-claim.component.scss'
})
export class AddNewClaimComponent {
  document = inject(DOCUMENT);
  @Output() create = new EventEmitter<AddNewRoleClaimModel>();
  addNewClaimModel: AddNewRoleClaimModel = {
    claimType: "",
    claimValue: "",
    roleId: ""
  }

  showAddFunction(){
    var form = this.document.getElementById("create-form");
    if (form){
      form.style.display = 'block';
    }
  }

  addClaim(){
    this.create.emit(this.addNewClaimModel);
    this.hideAddFunction();
  }

  hideAddFunction(){
    var form = this.document.getElementById("create-form");
    if (form){
      form.style.display = 'none';
    }
    this.addNewClaimModel.claimType = "";
    this.addNewClaimModel.claimValue = "";
  }

}

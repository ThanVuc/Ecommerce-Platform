import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UpdateRoleClaimModel } from '../../models/roles/update-role-claim-model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-update-role-claim',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './update-role-claim.component.html',
  styleUrl: './update-role-claim.component.scss'
})
export class UpdateRoleClaimComponent {
  updateRoleClaimModel: UpdateRoleClaimModel = {
    claimType: "",
    claimValue: ""
  }
  document = inject(DOCUMENT);
  @Output() update = new EventEmitter<UpdateRoleClaimModel>();


  showUpdateFunction(claimType: string, claimValue: string){
    this.updateRoleClaimModel.claimType = claimType;
    this.updateRoleClaimModel.claimValue = claimValue;
    var updateRoleElement = this.document.getElementById("update-form");
    if (updateRoleElement){
      updateRoleElement.style.display = 'block';
    }
  }

  updateClaim(){
    this.update.emit(this.updateRoleClaimModel);
    this.hideUpdateFunction();
  }

  hideUpdateFunction(){
    var updateRoleElement = this.document.getElementById("update-form");
    if (updateRoleElement){
      updateRoleElement.style.display = 'none';
    }
  }
}

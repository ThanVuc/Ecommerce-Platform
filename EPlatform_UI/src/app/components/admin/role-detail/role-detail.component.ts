import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { DOCUMENT } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AddNewClaimComponent } from "./add-new-claim/add-new-claim.component";
import { RoleResModel } from '../models/roles/roles-data-model';
import { AddNewRoleClaimModel } from '../models/roles/add-role-claim-model';
import { MessageComponent } from "../../../shares/reusable/message/message.component";
import { UpdateRoleClaimModel } from '../models/roles/update-role-claim-model';
import { UpdateRoleClaimComponent } from './update-role-claim/update-role-claim.component';
import { UpdateRoleComponent } from "../role/update-role/update-role.component";

@Component({
  selector: 'app-role-detail',
  standalone: true,
  imports: [AddNewClaimComponent, MessageComponent, UpdateRoleComponent, UpdateRoleClaimComponent],
  templateUrl: './role-detail.component.html',
  styleUrl: './role-detail.component.scss'
})
export class RoleDetailComponent implements OnInit {
  @ViewChild(MessageComponent) messager!: MessageComponent;
  @ViewChild(UpdateRoleClaimComponent) updater!: UpdateRoleClaimComponent;
  route = inject(ActivatedRoute);
  roleId!: string;
  claimId!: number;
  isLoading: boolean = true;

  ngOnInit(): void {
    this.route.params.subscribe( (params) => {
        this.roleId = params['id'];
        this.reloadPage(this.roleId);
      }
    )
  }
  adminSVC = inject(AdminService);
  document = inject(DOCUMENT);
  role: RoleResModel = {
    roleId: "",
    claims: [],
    roleName: ""
  };
  success: string = "success";
  fail: string = "fail";

  reloadPage(id: string){
    this.adminSVC.getRoleDetail(id)
    .subscribe({
      next: (res) => {
        this.isLoading = false;
        console.log(res.message);
        this.role = res.data;
      },
      error: (err) => {
        this.isLoading = false;
        console.log(err.error.message);
      }
    })
  }

  addRoleClaim(addRoleClaimModel: AddNewRoleClaimModel){
    addRoleClaimModel.roleId = this.roleId;
    this.adminSVC.addNewRoleClaim(addRoleClaimModel).subscribe({
      next: (res) => {
        this.isLoading = true;
        this.reloadPage(addRoleClaimModel.roleId);
        this.messager.showModal(this.success,res.message);
      },
      error: (err) => {
        console.log(err);
        this.messager.showModal(this.fail,err);
      }
    });
  }

  deleteRoleClaim(claimId: number){
    if (confirm("Are you sure to delete this claim?")){  
      this.adminSVC.deleteRoleClaim(this.roleId,claimId).subscribe({
        next: (res) => {
          this.isLoading = true;
          console.log(res.message);
          this.reloadPage(this.roleId);
          this.messager.showModal(this.success,res.message);
        },
        error: (err) => {
          console.log(err);
          this.messager.showModal(this.fail,err);
      }
      });
    }
  }

  showUpdateClaimForm(claimId: number, claimType: string, claimValue: string){
    this.claimId = claimId;
    this.updater.showUpdateFunction(claimType, claimValue);
  }

  updateRoleClaim(updateRoleClaimModel: UpdateRoleClaimModel){
    this.adminSVC.updateRoleClaim(this.roleId,this.claimId,updateRoleClaimModel)
    .subscribe({
      next: (res) => {
        this.isLoading = true;
        this.reloadPage(this.roleId);
        this.messager.showModal(this.success,"Update claim successc!");
      },
      error: (err) => {
        console.log(err);
        this.messager.showModal(this.fail,"Update claim fail!");
      }
    })
  }


}
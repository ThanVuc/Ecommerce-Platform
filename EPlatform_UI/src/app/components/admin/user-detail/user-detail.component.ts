import { Component, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { SetRoleComponent } from "./set-role/set-role.component";
import { UserDetailModel } from '../models/users/user-detail-model';
import { AdminService } from '../../services/admin.service';
import { DOCUMENT } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SetRoleStructure } from '../models/users/set-role-structure';
import { MessageComponent } from '../../../shares/reusable/message/message.component';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [SetRoleComponent, MessageComponent],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss'
})
export class UserDetailComponent implements OnInit {
  @ViewChild(MessageComponent) messenger!: MessageComponent;
  ngOnInit(): void {
    this.route.params.subscribe({
      next: (params) => {
        this.userId = params['id'];
        this.loadPage();
      }
    })
  }
  userDetailModel: UserDetailModel = {
    username: "",
    address: "",
    age: 0,
    avatarImageUrl: "",
    created: "",
    first: "",
    last: "",
    national: "",
    phoneNumber: "",
    roles: [],
    gender: true
  };
  adminSVC = inject(AdminService);
  document = inject(DOCUMENT);
  route = inject(ActivatedRoute);
  userId: string = "";

  isLoading = true;

  loadPage(){
    this.adminSVC.getUser(this.userId).subscribe({
      next: (res) => {
        this.userDetailModel = res.data;
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  setRole(roleIdList: string[]){
    this.adminSVC.setRole(this.userId,roleIdList).subscribe({
      next: (res) => {
        this.loadPage();
        this.messenger.showModal("success","Set the roles succesful!");
      },
      error: (error) => {
        this.messenger.showModal("fail",error);
      }
    })
  }

}

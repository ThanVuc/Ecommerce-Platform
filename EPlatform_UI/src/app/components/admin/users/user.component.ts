import { Component, inject, Input, OnInit, ViewChild } from '@angular/core';
import { PaginationComponent } from '../../../shares/reusable/pagination/pagination.component';
import { PageModel } from '../../models/PageModel';
import { AdminService } from '../../services/admin.service';
import { DatePipe, DOCUMENT } from '@angular/common';
import { map } from 'rxjs';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';
import { UserModel } from '../models/users/user-model';
import { MessageComponent } from '../../../shares/reusable/message/message.component';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CreateUserComponent } from "./create-user/create-user.component";
import { CreateUserModel } from '../models/users/create-user-model';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [PaginationComponent, DatePipe, FormsModule, RouterLink, MessageComponent, CreateUserComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss'
})
export class UserComponent implements OnInit {
  @ViewChild(PaginationComponent) paginator!: PaginationComponent;
  @ViewChild(MessageComponent) messager!: MessageComponent;
  
  ngOnInit(): void {
    this.initPage();
  }
  adminSVC = inject(AdminService);
  document = inject(DOCUMENT);


  pageIndex: number = 1;
  limit: number = 10;
  totalItem: number = 1;
  searchString: string = "";

  isLoading = true;

  users: UserModel[] = [];
  

  initPage(){
    this.adminSVC.getUsers(1,1,this.searchString).pipe(
      map(res => {
        const paginationInfo = res.headers.get("X-Pagination");
          let data: PaginationInfoModel | null = null;
          if (paginationInfo != null){
            data = JSON.parse(paginationInfo);
          }

          if (data?.TotalItem){
            this.totalItem = data.TotalItem;
          }

          return res.body;
      })
    ).subscribe({
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  loadPage(pageModel: PageModel){
    this.pageIndex = pageModel.pageIndex;
    this.limit = pageModel.pageSize;
    this.adminSVC.getUsers(this.pageIndex,this.limit,this.searchString).pipe(
      map(res => {
        const paginationInfo = res.headers.get("X-Pagination");
          let data: PaginationInfoModel | null = null;
          if (paginationInfo != null){
            data = JSON.parse(paginationInfo);
          }

          if (data?.TotalItem){
            this.totalItem = data.TotalItem;
          }

          return res.body;
      })
    ).subscribe({
      next: (res) => {
        if (res?.data){
          this.users = res.data;
          this.isLoading = false;
        };
        
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  changeStatus(id: string){
    if (confirm("Are you sure to change user status?")){
      this.adminSVC.changeUserStatus(id).subscribe({
        next: (res) => {
          console.log(res.message);
          this.paginator.pageChange(this.pageIndex);
        },
        error: (err) => {
          console.log(err);
        }
      })
    }
  }

  createUser(createUserModel: CreateUserModel){
    this.adminSVC.createUserModel(createUserModel).subscribe({
      next: (res) => {
        this.paginator.pageChange(this.pageIndex);
        this.messager.showModal("success","Create User Success");
      },
      error: (err) => {
        console.log(err);
        this.messager.showModal("fail",err);
      }
    })
  }
}



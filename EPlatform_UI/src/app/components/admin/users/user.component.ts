import { Component, inject, Input, OnInit, ViewChild } from '@angular/core';
import { PaginationComponent } from '../../../shares/reusable/pagination/pagination.component';
import { PageModel } from '../../models/PageModel';
import { AdminService } from '../../services/admin.service';
import { DatePipe, DOCUMENT } from '@angular/common';
import { map } from 'rxjs';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';
import { UserModel } from '../models/users/user-model';
import { MessageComponent } from '../../../shares/reusable/message/message.component';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [PaginationComponent, DatePipe],
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
  limit: number = 1;
  totalItem: number = 1;

  users: UserModel[] = [];

  initPage(){
    this.adminSVC.getUsers(1,1).pipe(
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

      }
    });
  }

  loadPage(pageModel: PageModel){
    this.pageIndex = pageModel.pageIndex;
    this.limit = pageModel.pageSize;
    this.adminSVC.getUsers(this.pageIndex,this.limit).pipe(
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
}



import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { PaginationComponent } from "../../../shares/reusable/pagination/pagination.component";
import { PageModel } from '../../models/PageModel';
import { AdminService } from '../../services/admin.service';
import { ApiResModel } from '../../models/api-res-model';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';
import { map } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { CreateRoleModel } from '../models/roles/create-role-model';
import { MessageComponent } from "../../../shares/reusable/message/message.component";
import { DeleteRoleModel } from '../models/roles/delete-role-modal';
import { UpdateRoleModel } from '../models/roles/update-role-model';
import { RouterLink } from '@angular/router';
import { CreateRoleComponent } from "./create-role/create-role.component";
import { RoleResModel } from '../models/roles/roles-data-model';
import { UpdateRoleComponent } from './update-role/update-role.component';

@Component({
  selector: 'app-role',
  standalone: true,
  imports: [PaginationComponent, FormsModule, MessageComponent, RouterLink, CreateRoleComponent, UpdateRoleComponent],
  templateUrl: './role.component.html',
  styleUrl: './role.component.scss'
})
export class RoleComponent implements OnInit {
  @ViewChild(PaginationComponent) paginator!: PaginationComponent;
  @ViewChild(MessageComponent) messager!: MessageComponent;
  @ViewChild(UpdateRoleComponent) updater!: UpdateRoleComponent;
  ngOnInit(): void {
    let pageModel: PageModel = {
      pageIndex: this.pageIndex,
      pageSize: this.limit
    };
    this.initLoad();
  }
  isLoading: boolean = true;
  adminSVC = inject(AdminService);
  document = inject(DOCUMENT);
  roleId: string = "";
  roles: RoleResModel[] = [];
  totalItem!: number;
  limit: number = 10;
  pageIndex: number = 1;

  updateRoleModel: UpdateRoleModel = {
    name: "",
    roleId: ""
  }
  message: string = "";
  messageStatus: string = "";

  initLoad(){
    this.adminSVC.getRoles(1,1)
    .pipe(
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
    )
    .subscribe({
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  loadPage(pageModel: PageModel){
    this.pageIndex = pageModel.pageIndex;
    this.adminSVC.getRoles(pageModel.pageIndex, pageModel.pageSize)
    .pipe(
      map(res => {
        const paginationInfo = res.headers.get("X-Pagination");
          let data: PaginationInfoModel | null = null;
          if (paginationInfo != null){
            data = JSON.parse(paginationInfo);
          }

          if (data?.TotalItem){
            this.totalItem = this.totalItem;
          }
          return res.body;
      })
    )
    .subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res?.status == 200){
          this.roles = res.data;
        }
      },
      error: (err) => {
        console.error(err);
      }
    })
  }

  createRole(createRoleModel: CreateRoleModel){
    this.adminSVC.createRole(createRoleModel)
    .subscribe({
      next: (res) => {
        console.log(res.message);
        var createForm = this.document.getElementById("create-form");
        this.isLoading = true;
        this.paginator.pageChange(this.pageIndex);
        this.messager.showModal("success","Create Role Successful");
      },
      error: (err) => {
        console.log(err);
        this.messager.showModal("fail",err);
      }
    });
  }

  deleteRole(id: string){
    if (confirm("Do you really want to delete this role?")){
      this.adminSVC.deleteRole(id).subscribe({
        next: (res) => {
          console.log(res.message);
          this.isLoading = true;
          this.paginator.pageChange(this.pageIndex);
          this.messager.showModal("success","Delete Role Successful");
        },
        error: (err) => {
          console.log(err);
          this.messager.showModal("fail",err);
        }
      })
    }
  }

  showUpdateRole(roleId: string, roleName: string){
    this.updater.showUpdateRole(roleId,roleName);
  }

  updateRole(updateRoleModel: UpdateRoleModel){
    this.adminSVC.updateRole(updateRoleModel.roleId,updateRoleModel)
    .subscribe({
      next: (res) => {
        this.isLoading = true;
        console.log(res.message);
        this.paginator.pageChange(this.pageIndex);
        this.messager.showModal("success","Update Role Successful");
      },
      error: (err) => {
        console.log(err);
        this.messager.showModal("fail",err);
      }
    })
  }


}

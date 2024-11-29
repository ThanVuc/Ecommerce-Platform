import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { ApiResModel } from '../models/api-res-model';
import { CreateRoleModel } from '../admin/models/roles/create-role-model';
import { DeleteRoleModel } from '../admin/models/roles/delete-role-modal';
import { UpdateRoleModel } from '../admin/models/roles/update-role-model';
import { RoleResModel } from '../admin/models/roles/roles-data-model';
import { AddNewRoleClaimModel } from '../admin/models/roles/add-role-claim-model';
import { DeleteRoleClaimModel } from '../admin/models/roles/delete-role-claim-model';
import { UpdateRoleClaimModel } from '../admin/models/roles/update-role-claim-model';
import { UserModel } from '../admin/models/users/user-model';
import { LockOrUnlockModel } from '../admin/models/users/lock-or-unlock-model';
import { CreateUserModel } from '../admin/models/users/create-user-model';
import { UserDetailModel } from '../admin/models/users/user-detail-model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private http: HttpClient) { }

  getRoles(pageIndex: number | null, pageSize: number | null) : Observable<HttpResponse<ApiResModel<RoleResModel[]>>> {
    if (pageIndex == null || pageSize == null){
      return this.http.get<ApiResModel<RoleResModel[]>>(environment.GetRoles,{observe: 'response'});
    }
    return this.http.get<ApiResModel<RoleResModel[]>>(environment.GetRoles + `?PageNumber=${pageIndex}&PageSize=${pageSize}`,{observe: 'response'});
  }

  createRole(createRoleModel: CreateRoleModel) : Observable<ApiResModel<object>>{
    return this.http.post<ApiResModel<object>>(environment.CreateRole,createRoleModel);
  }

  deleteRole(id: string) : Observable<ApiResModel<object>>{
    console.log(environment.DeleteRole + id);
    return this.http.delete<ApiResModel<object>>(environment.DeleteRole+id);
  }

  updateRole(id: string, updateRoleModel: UpdateRoleModel) : Observable<ApiResModel<object>>{
    return this.http.put<ApiResModel<object>>(environment.UpdateRole+id,updateRoleModel);
  }

  getRoleDetail(id: string) : Observable<ApiResModel<RoleResModel>>{
    return this.http.get<ApiResModel<RoleResModel>>(environment.GetRoleById+id);
  }

  addNewRoleClaim(addRoleClaimModel: AddNewRoleClaimModel) : Observable<ApiResModel<object>>{
    return this.http.post<ApiResModel<object>>(environment.AddRoleClaim+addRoleClaimModel.roleId+"/add-claim",addRoleClaimModel);
  }

  deleteRoleClaim(roleId: string, claimId:number) : Observable<ApiResModel<object>> {
    return this.http.delete<ApiResModel<object>>(environment.DeleteRoleClaim+roleId+"/delete-claim/"+claimId);
  }

  updateRoleClaim(roleId: string, claimId: number, updateRoleClaimModel: UpdateRoleClaimModel) : Observable<ApiResModel<object>>{
    return this.http.put<ApiResModel<object>>(environment.UpdateRoleClaim+roleId+"/update-claim/"+claimId,updateRoleClaimModel);
  }

  getUsers(pageIndex: number, pageSize: number, searchString: string) : Observable<HttpResponse<ApiResModel<UserModel[]>>>{
    return this.http.get<ApiResModel<UserModel[]>>(environment.GetUsers+ `?PageNumber=${pageIndex}&PageSize=${pageSize}&SearchString=${searchString}`, {observe: "response"});
  }

  changeUserStatus(id: string) : Observable<ApiResModel<Boolean>>{
    let lockOrUnlockModel: LockOrUnlockModel = {
      id: id
    };
    return this.http.post<ApiResModel<boolean>>(environment.LockOrUnlockUser,lockOrUnlockModel);
  }

  createUserModel(creatUserModel: CreateUserModel):Observable<ApiResModel<object>>{
    return this.http.post<ApiResModel<object>>(environment.CreateUser,creatUserModel);
  }

  getUser(id: string) : Observable<ApiResModel<UserDetailModel>>{
    return this.http.get<ApiResModel<UserDetailModel>>(environment.GetUserById+id);
  }

  setRole(userId: string, rolesId: string[]) : Observable<ApiResModel<object>>{
    let model = {
      rolesId: rolesId
    };
    return this.http.post<ApiResModel<object>>(environment.SettingRoleOfUser + userId + "/setting-role-for-user",model);
  }
}

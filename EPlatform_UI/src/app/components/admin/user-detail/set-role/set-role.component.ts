import { DOCUMENT } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SetRoleStructure } from '../../models/users/set-role-structure';
import { AdminService } from '../../../services/admin.service';
import { ActivatedRoute } from '@angular/router';
import { emit } from 'process';

@Component({
  selector: 'app-set-role',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './set-role.component.html',
  styleUrl: './set-role.component.scss'
})
export class SetRoleComponent implements OnInit {
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.userId = params['id'];
    })
  }
  @Output() set = new EventEmitter<string[]>();

  document = inject(DOCUMENT);
  adminSVC = inject(AdminService);
  route = inject(ActivatedRoute);

  roleList: SetRoleStructure[] = [];
  userId: string = "";
  

  getAllRole(){
    this.roleList = [];
    this.adminSVC.getRoles(null,null).subscribe({
      next: (res) => {
        if (res.body?.data){
          let roles = res.body.data;
          roles.forEach(role => {
            let roleElement: SetRoleStructure = {
              roleId: role.roleId,
              roleName: role.roleName,
              isChecked: false
            }
            this.roleList.push(roleElement);

            this.getRolesOfUserAndMatch();
          });
        }
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  getRolesOfUserAndMatch(){
    this.adminSVC.getUser(this.userId).subscribe({
      next: (res) => {
        var roles = res.data.roles
        roles.forEach((roleName) => {
          this.roleList.forEach((r) => {
            if (r.roleName == roleName){
              r.isChecked = true;
            }
          });
        });
      }
    });
  }

  showSetRoleForm(){
    var form = this.document.getElementById("set-role-form");
    this.getAllRole();
    if (form){
      form.style.display = 'block';
    }
  }

  setRole(){
    let roleIdList: string[] = [];
    this.roleList.forEach((role) => {
      if (role.isChecked){
        roleIdList.push(role.roleId);
      }
    });
    this.set.emit(roleIdList);

    this.hideSetRoleForm();
  }

  hideSetRoleForm(){
    var form = this.document.getElementById("set-role-form");
    if (form){
      form.style.display = 'none';
    }
  }
}

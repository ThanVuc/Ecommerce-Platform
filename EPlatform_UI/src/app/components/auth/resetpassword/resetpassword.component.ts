import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { AdminlayoutComponent } from "../../../shares/layouts/adminlayout/adminlayout.component";
import { AuthHeaderComponent } from "../../../shares/reusable/auth-header/auth-header.component";
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ResetPasswordRequestModel } from '../models/ResetPasswordRequestModel';
import { AuthService } from '../../services/auth.service';
import { catchError, of } from 'rxjs';
import { error } from 'console';

@Component({
  selector: 'app-resetpassword',
  standalone: true,
  imports: [FormsModule, RouterLink, AuthHeaderComponent],
  templateUrl: './resetpassword.component.html',
  styleUrl: './resetpassword.component.scss'
})
export class ResetpasswordComponent implements OnInit {
  titleService = inject(Title);
  authSVC = inject(AuthService);
  title = "Reset Password";
  extraErr = "";
  resetPasswordModel: ResetPasswordRequestModel = {
    oldPassword: "",
    newPassword: "",
    confirmNewPassword: ""
  }
  ngOnInit(): void {
    this.titleService.setTitle(this.title);
  }

  resetPassword(){
    if (this.resetPasswordModel.newPassword != this.resetPasswordModel.confirmNewPassword){
      this.extraErr = "The confirm passowrd has to equal with new password";
      return;
    }
    this.authSVC.resetPassword(this.resetPasswordModel).pipe(
      catchError(err => {
        this.extraErr = err;
        return of(null);
      })
    ).subscribe(res => {
      if (res && res.status === 200){
        alert(res.message);
      }
    });
  }
}

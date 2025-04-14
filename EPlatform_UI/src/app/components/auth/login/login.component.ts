import { AfterContentChecked, AfterViewInit, ChangeDetectorRef, Component, inject, Input, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { SignInRequestModel } from '../models/SignInRequestModel';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { catchError, of } from 'rxjs';
import { PassDataService } from '../../services/pass-data.service';
import { AuthHeaderComponent } from "../../../shares/reusable/auth-header/auth-header.component";
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
import { JwtTokenModel } from '../models/JwtTokenModel';
import { ApiModel } from '../../models/ApiModel';
import { ForgotPasswordRequestModel } from '../models/ForgotPasswordRequestMode';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, AuthHeaderComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  authService = inject(AuthService);
  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);
  titleService = inject(Title);
  document = inject(DOCUMENT);
  http = inject(HttpClient);
  title = "Sign In";
  signInModel: SignInRequestModel = {
    username: "",
    password: ""
  }

  forgotPasswordModel: ForgotPasswordRequestModel = {
    email: "",
    verifyCode: ""
  }

  isRemember: boolean = false;
  extraErr:string | null = null;
  returnUrl: string  = "/";
  constructor(){
  }

  ngOnInit(): void {
    this.titleService.setTitle(this.title);
    this.returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || '/';
    // if (this.document.defaultView?.localStorage){
    //   this.authService.signOut();
    // }
  }

  signIn(){
    this.authService.signIn(this.signInModel).pipe(
      catchError((err) => {
        this.extraErr = err;
        return of(null)
      })
    ).subscribe((res) => {
      if (res?.status == 200){
        this.router.navigateByUrl(decodeURIComponent(this.returnUrl));
      }
    })
  }

  verifyFailCount: number = 0;
  forgotFailErr: string = "";

  showForgotPassword(){
    var formModal = this.document.getElementById("form-modal");
    var forgotPassword = this.document.getElementById("forgot-password");
    if (formModal && forgotPassword){
      formModal.style.display = 'block';
      forgotPassword.style.display = 'block';
      this.forgotFailErr = "";
    }
  }

  recoveryPassword = () => {
    this.authService.recoveryPassword(this.forgotPasswordModel)
    .pipe(
      catchError(err => {
        this.forgotFailErr = err;
        return of(null);
      })
    )
    .subscribe((res) => {
      if (res){
        if (res.status === 200){
          this.forgotFailErr = "";
          let forgotPassword = this.document.getElementById("forgot-password");
          let verifyConfirm = this.document.getElementById("verify-confirm");
          if (forgotPassword && verifyConfirm){
            forgotPassword.style.display = 'none';
            verifyConfirm.style.display = 'block';
          }
        }
      }
    })
  }

  confirmVerifyCode(){
    this.http.post<ApiModel<JwtTokenModel>>(environment.ConfirmRecoveryPassword,this.forgotPasswordModel)
    .pipe(
      catchError((err:ApiModel<JwtTokenModel>) => {
        this.verifyFailCount++;

        if (this.verifyFailCount == 5){
          this.verifyFailCount = 0;
          this.backToRegister(true);
        }

        return of(null);
      })
    ).subscribe((res) => {
      if (res?.data){
        this.verifyFailCount = 0;
        this.router.navigate(["/auth/reset-password"]);
      }
    });
  }

  backToRegister(isFailCount5Times:boolean = false){
    const formModal = this.document.getElementById("form-modal");
    const verifyConfirm = this.document.getElementById("verify-confirm");
    const forgotPassword = this.document.getElementById("forgot-password");
    
    const verifyWrong5times = this.document.getElementById("verify-wrong-5-times");
    if (formModal && verifyConfirm && forgotPassword){
      formModal.style.display = 'none';
      verifyConfirm.style.display = 'none';
      forgotPassword.style.display = 'none';
    }

    if (isFailCount5Times){
      if (verifyWrong5times){
        verifyWrong5times.style.display = 'block';
      }
    }

  }
}

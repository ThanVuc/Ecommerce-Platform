import { AfterContentChecked, AfterViewInit, ChangeDetectorRef, Component, inject, Input, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { SignInRequestModel } from '../models/SignInRequestModel';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { catchError, of } from 'rxjs';
import { TokenService } from '../../services/token.service';
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
  tokenService = inject(TokenService);
  router = inject(Router);
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
    otp: ""
  }

  isRemember: boolean = false;
  extraErr:string | null = null;
  constructor(){

  }

  ngOnInit(): void {
    this.titleService.setTitle(this.title);
  }

  signIn(){
    if (this.isRemember){
      localStorage.setItem("isRemeber","1");
    }

    this.authService.signIn(this.signInModel).pipe(
      catchError((err) => {
        this.extraErr = err.error.message;
        return of(null)
      })
    ).subscribe((res) => {
      if (res?.data != null){
        this.tokenService.saveJWTToken(res?.data);
        this.router.navigate(["/"]);
      }
    })
  }

  otpFailCount: number = 0;
  forgotFailErr: string = "";

  showForgotPassword(){
    var formModal = this.document.getElementById("form-modal");
    if (formModal){
      formModal.style.display = 'block';
      this.forgotFailErr = "";
    }
  }

  recoveryPassword = () => {
    this.authService.recoveryPassword(this.forgotPasswordModel)
    .pipe(
      catchError(err => {
        this.forgotFailErr = err.error.message;
        return of(null);
      })
    )
    .subscribe((res) => {
      if (res){
        if (res.status === 200){
          this.forgotFailErr = "";
          let forgotPassword = this.document.getElementById("forgot-password");
          let otpConfirm = this.document.getElementById("otp-confirm");
          if (forgotPassword && otpConfirm){
            forgotPassword.style.display = 'none';
            otpConfirm.style.display = 'block';
          }
        }
      }
    })
  }

  confirmOTP(){
    this.http.post<ApiModel<JwtTokenModel>>(environment.ConfirmRecoveryPassword,this.forgotPasswordModel)
    .pipe(
      catchError((err:ApiModel<JwtTokenModel>) => {
        this.otpFailCount++;

        if (this.otpFailCount == 5){
          this.otpFailCount = 0;
          this.backToRegister(true);
        }

        return of(null);
      })
    ).subscribe((res) => {
      if (res?.data){
        this.otpFailCount = 0;
        this.tokenService.saveJWTToken(res.data);
        this.router.navigate(["/auth/reset-password"]);
      }
    });
  }

  backToRegister(isFailCount5Times:boolean = false){
    const formModal = this.document.getElementById("form-modal");
    const otpWrong5Times = this.document.getElementById("otp-wrong-5-times");
    if (formModal){
      formModal.style.display = 'none';
    }

    if (isFailCount5Times){
      if (otpWrong5Times){
        otpWrong5Times.style.display = 'block';
      }
    }

  }
}

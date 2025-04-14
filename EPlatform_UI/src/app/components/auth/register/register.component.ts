import { AfterContentChecked, AfterViewInit, ChangeDetectorRef, Component, Directive, Inject, inject, OnInit } from '@angular/core';
import { SignUpRequestModel } from '../models/SignUpRequestModel';
import { FormControl, FormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { catchError, defaultIfEmpty, of, throwError } from 'rxjs';
import { ApiModel } from '../../models/ApiModel';
import { JwtTokenModel } from '../models/JwtTokenModel';
import { throws } from 'assert';
import { AsyncLocalStorage } from 'async_hooks';
import { LocalStorageService } from '../../services/local-storage.service';
import { Router, RouterLink } from '@angular/router';
import { PhoneDirective } from '../../validator/PhoneValidator';
import { CommonModule, DOCUMENT } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { PassDataService } from '../../services/pass-data.service';
import { AuthHeaderComponent } from "../../../shares/reusable/auth-header/auth-header.component";
import { register } from 'module';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, PhoneDirective, RouterLink, AuthHeaderComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  ngOnInit(): void {
    this.titleService.setTitle(this.title);
  }
  title = "Sign Up";
  signUpModel: SignUpRequestModel = {
    username: "",
    password: "",
    address: "",
    first: "",
    last: "",
    phoneNumber: "",
    confirmPassword: "",
    verifyCode: ""
  }
  titleService = inject(Title)
  authService = inject(AuthService);
  router = inject(Router);
  extraErr:string | null = null;
  passDataService = inject(PassDataService);
  document = inject(DOCUMENT)
  http = inject(HttpClient);

  signUp = () => {
    if (this.signUpModel.password !== this.signUpModel.confirmPassword){
      this.extraErr = "ConfirPassword have to like Password";
      return;
    }
    this.authService.signUp(this.signUpModel)
    .pipe(
      catchError((err) => {
        this.extraErr = err;
        return of(null)
      })
    ).subscribe((res) => {
      if (res !== null){
        this.verifyFailCount = 0;
        const formModal = this.document.getElementById("form-modal");
        if (formModal){
          formModal.style.display = 'block';
        }
      }
    })
  }

  verifyFailCount: number = 0;

  confirmVerify(){
    this.http.post<ApiModel<JwtTokenModel>>(environment.RegisterConfirmAPI,this.signUpModel)
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
      if (res?.status == 201){
        this.verifyFailCount = 0;
        this.router.navigate(["/"]);
      }
    });
  }

  backToRegister(isFailCount5Times:boolean = false){
    const formModal = this.document.getElementById("form-modal");
    const verifyWrong5Times = this.document.getElementById("verify-wrong-5-times");
    if (formModal){
      formModal.style.display = 'none';
    }

    if (isFailCount5Times){
      if (verifyWrong5Times){
        verifyWrong5Times.style.display = 'block';
      }
    }

  }
}

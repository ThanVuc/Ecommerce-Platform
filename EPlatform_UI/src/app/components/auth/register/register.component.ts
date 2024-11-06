import { AfterContentChecked, AfterViewInit, ChangeDetectorRef, Component, Directive, Inject, inject, OnInit } from '@angular/core';
import { SignUpRequestModel } from '../models/SignUpRequestModel';
import { FormControl, FormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { catchError, of, throwError } from 'rxjs';
import { ApiModel } from '../../models/ApiModel';
import { JwtTokenModel } from '../models/JwtTokenModel';
import { throws } from 'assert';
import { AsyncLocalStorage } from 'async_hooks';
import { LocalStorageService } from '../../services/local-storage.service';
import { TokenService } from '../../services/token.service';
import { Router, RouterLink } from '@angular/router';
import { PhoneDirective } from '../../validator/PhoneValidator';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { PassDataService } from '../../services/pass-data.service';
import { AuthHeaderComponent } from "../../../shares/reusable/auth-header/auth-header.component";

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
    confirmPassword: ""
  }
  titleService = inject(Title)
  authService = inject(AuthService);
  tokenService = inject(TokenService);
  router = inject(Router);
  extraErr:string | null = null;
  passDataService = inject(PassDataService);
  signUp = () => {
    if (this.signUpModel.password !== this.signUpModel.confirmPassword){
      this.extraErr = "ConfirPassword have to like Password";
      return;
    }
    this.authService.signUp(this.signUpModel)
    .pipe(
      catchError((err) => {
        this.extraErr = err.error.message;
        return of(null)
      })
    ).subscribe((res) => {
      if (res?.data != null){
        this.tokenService.saveJWTToken(res?.data);
        this.router.navigate(["/"])
      }
    })
  }
}

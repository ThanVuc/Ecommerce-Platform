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
  title = "Sign In";
  signInModel: SignInRequestModel = {
    username: "",
    password: ""
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
}

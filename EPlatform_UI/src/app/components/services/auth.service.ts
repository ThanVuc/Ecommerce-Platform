import { Injectable } from '@angular/core';
import { SignUpRequestModel } from '../auth/models/SignUpRequestModel';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiModel } from '../models/ApiModel';
import { JwtTokenModel } from '../auth/models/JwtTokenModel';
import { environment } from '../../../environments/environment.development';
import { SignInRequestModel } from '../auth/models/SignInRequestModel';
import { LocalStorageService } from './local-storage.service';
import { ResetPasswordRequestModel } from '../auth/models/ResetPasswordRequestModel';
import { ForgotPasswordRequestModel } from '../auth/models/ForgotPasswordRequestMode';
import { ApiResModel } from '../models/api-res-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private localStorage: LocalStorageService) { }
  
  signUp(signUpModel: SignUpRequestModel): Observable<ApiModel<JwtTokenModel>> {
    return this.http.post<ApiModel<JwtTokenModel>>(environment.SignUpAPI,signUpModel)
  }

  signIn(signInModel: SignInRequestModel): Observable<ApiModel<JwtTokenModel>>{
    return this.http.post<ApiModel<JwtTokenModel>>(environment.SignInAPI,signInModel);
  }

  signOut(){
    return this.http.post<ApiModel<object>>(environment.RevokeJWTTokenAPI,{}).subscribe({
      next: (res) => {
        
      },
      error: (err) => {
        console.error("Error signing out: ", err);
      }
    });
  }

  resetPassword(resetPasswordModel: ResetPasswordRequestModel): Observable<ApiModel<object>>{
    return this.http.post<ApiModel<object>>(environment.ResetPasswordAPI,resetPasswordModel);
  }

  recoveryPassword(forgotPasswordModel: ForgotPasswordRequestModel): Observable<ApiModel<object>>{
    return this.http.post<ApiModel<object>>(environment.ForgotPasswordAPI,forgotPasswordModel);
  }

  IsAuthenticatedOrRefresh(): Observable<ApiModel<boolean>> {
    return this.http.post<ApiResModel<boolean>>(environment.RefreshJWTTokenAPI,null,
      { withCredentials: true } // Ensure cookies are sent with the request
    );
  }

  checkRole(role: string): Observable<ApiModel<boolean>> {
    return this.http.post<ApiModel<boolean>>(environment.checkRoleAPI, { role: role }, 
      { withCredentials: true}
    );
  }
}

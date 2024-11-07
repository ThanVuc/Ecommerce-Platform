import { Injectable } from '@angular/core';
import { SignUpRequestModel } from '../auth/models/SignUpRequestModel';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiModel } from '../models/ApiModel';
import { JwtTokenModel } from '../auth/models/JwtTokenModel';
import { environment } from '../../../environments/environment.development';
import { SignInRequestModel } from '../auth/models/SignInRequestModel';
import { LocalStorageService } from './local-storage.service';

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
    let refreshToken = this.localStorage.getValue("RefreshToken");

    if (refreshToken){
      this.http.post(environment.RevokeJWTTokenAPI,null).subscribe((res) => {
        console.log("Logut Successful")
      });
    }

    this.localStorage.removeValue('AccessToken');
    this.localStorage.removeValue('RefreshToken');
  }
}

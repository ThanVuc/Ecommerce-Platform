import { Injectable } from '@angular/core';
import { JwtTokenModel } from '../auth/models/JwtTokenModel';
import { LocalStorageService } from './local-storage.service';
import { SessionService } from './session.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  constructor(
    private localStorageService: LocalStorageService,
    private localStorage: LocalStorageService,
    private jwtHelper: JwtHelperService
  ) { }
  saveJWTToken(tokenModel: JwtTokenModel){
    this.localStorageService.setValue("AccessToken",tokenModel.accessToken);
    this.localStorageService.setValue("RefreshToken",tokenModel.refreshToken);
  }
  
  isAuthenicated(): boolean{
    const accessToken = this.localStorage.getValue("AccessToken");
    let isAuthenicated = false;
    if (accessToken && !this.jwtHelper.isTokenExpired(accessToken)){
      isAuthenicated = true;
    }
    return isAuthenicated;
  }

  getRole(){
    const accessToken = this.localStorage.getValue("AccessToken");
    const roleKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    if (accessToken){
      const decode = this.jwtHelper.decodeToken(accessToken);
      console.log(decode);
      return decode[roleKey];
    }
    return null;
  }
}

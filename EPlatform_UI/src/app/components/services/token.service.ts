import { Injectable } from '@angular/core';
import { JwtTokenModel } from '../auth/models/JwtTokenModel';
import { LocalStorageService } from './local-storage.service';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  constructor(
    private localStorageService: LocalStorageService,
  ) { }
  saveJWTToken(tokenModel: JwtTokenModel){
    this.localStorageService.setValue("AccessToken",tokenModel.accessToken);
    this.localStorageService.setValue("RefreshToken",tokenModel.refreshToken);
  }
}

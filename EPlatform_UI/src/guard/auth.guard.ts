import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, Observable, of } from 'rxjs';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { JwtTokenModel } from '../app/components/auth/models/JwtTokenModel';
import { ApiModel } from '../app/components/models/ApiModel';
import { environment } from '../environments/environment.development';
import { TokenService } from '../app/components/services/token.service';
import { resolve } from 'path';
import { rejects } from 'assert';
import { ApiResModel } from '../app/components/models/api-res-model';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate{
  router = inject(Router);
  jwtHelper = inject(JwtHelperService);
  localStorage = inject(LocalStorageService);
  http = inject(HttpClient);
  tokenService = inject(TokenService);

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean>{
    const accessToken = this.localStorage.getValue("AccessToken");
    if (accessToken && !this.jwtHelper.isTokenExpired(accessToken)){
      return true;
    }
    const isRefresh = await this.tryRefreshToken(accessToken);
    if (!isRefresh) { 
      this.router.navigate(["auth"], { replaceUrl: true});
    }
    return isRefresh;
  }

  private async tryRefreshToken(accessToken: string | null | undefined) : Promise<boolean>
  {
    const refreshToken = this.localStorage.getValue("RefreshToken");
    if (!accessToken || !refreshToken){
      return false;
    }

    let isRefresh: boolean = true;
    let jwtToken: JwtTokenModel = {
      accessToken: accessToken,
      refreshToken: refreshToken
    }

    const refreshRes = await new Promise<JwtTokenModel>((resolve,reject) => {
      this.http.post<ApiResModel<JwtTokenModel>>(environment.RefreshJWTTokenAPI,jwtToken,{
        headers: new HttpHeaders({
          "Content-Type": "application/json"
        })
      }).subscribe({
        next: (res) => resolve(res.data),
        error: (_) => {
          reject; 
          isRefresh = false;
          this.router.navigate(["auth"], { replaceUrl: true});
        }
      })
    })

    this.localStorage.setValue("AccessToken",refreshRes.accessToken);
    this.localStorage.setValue("RefreshToken",refreshRes.refreshToken);
    return isRefresh;
  }

}


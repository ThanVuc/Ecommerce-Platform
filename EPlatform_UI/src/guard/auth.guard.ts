import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, Observable, of } from 'rxjs';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { HttpClient } from '@angular/common/http';
import { JwtTokenModel } from '../app/components/auth/models/JwtTokenModel';
import { ApiModel } from '../app/components/models/ApiModel';
import { environment } from '../environments/environment.development';
import { TokenService } from '../app/components/services/token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate{
  router = inject(Router)
  jwtHelper = inject(JwtHelperService)
  localStorage = inject(LocalStorageService)
  http = inject(HttpClient)
  tokenService = inject(TokenService)
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean{
    const accessToken = this.localStorage.getValue("AccessToken");
    if (accessToken && !this.jwtHelper.isTokenExpired(accessToken)){
      return true;
    }
    let isRefresh = false;

    isRefresh = this.tryRefreshToken(accessToken);

    if (!isRefresh){
      this.router.navigate(["/auth"])
    }
    return isRefresh;
  }

  private tryRefreshToken(accessToken: string | null | undefined) : boolean{
    const refreshToken = this.localStorage.getValue("RefreshToken");
    if (!accessToken || !refreshToken){
      return false;
    }

    let isRefresh: boolean = true;
    let jwtToken: JwtTokenModel = {
      accessToken: accessToken,
      refreshToken: refreshToken
    }

    this.http.post<ApiModel<JwtTokenModel>>(environment.RefreshJWTTokenAPI,jwtToken)
    .pipe(
      catchError((err) => {
        console.error(err.error.message);
        isRefresh = false;
        return of(null);
      })
    )
    .subscribe((res) => {
      if (res?.data){
        this.tokenService.saveJWTToken(res.data);
      }
    });
    return isRefresh;
  }
}


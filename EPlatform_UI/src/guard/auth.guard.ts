import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, Observable, of } from 'rxjs';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { JwtTokenModel } from '../app/components/auth/models/JwtTokenModel';
import { ApiModel } from '../app/components/models/ApiModel';
import { environment } from '../environments/environment.development';
import { resolve } from 'path';
import { rejects } from 'assert';
import { ApiResModel } from '../app/components/models/api-res-model';
import { AuthService } from '../app/components/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate{
  router = inject(Router);
  localStorage = inject(LocalStorageService);
  authSVC = inject(AuthService);


  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean>{
    const isAuthenticatedOrRefreshed = await this.tryRefreshToken();
    if (!isAuthenticatedOrRefreshed) {
      this.router.navigate(["auth"], { queryParams: {"returnUrl": "/"} , replaceUrl: true});
    }
    return isAuthenticatedOrRefreshed;
  }

  private async tryRefreshToken(): Promise<boolean> {
    try {
      const refreshRes = await new Promise<boolean>((resolve, reject) => {
        this.authSVC.IsAuthenticatedOrRefresh().subscribe({
          next: (res) => {
            if (res.status == 200) {
              resolve(res.data);
            } else {
              reject(res.message);
            }
          },
          error: (err) => {
            reject(err);
          }
        });
      });
      return refreshRes;
    } catch (error) {
      console.error("Error refreshing token: ", error);
      return false;
    }
  }
}


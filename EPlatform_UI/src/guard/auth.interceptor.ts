import { HttpContext, HttpContextToken, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.context.get(IS_PUBLIC)){
    return next(req);
  }
  console.log("aaaaaaaaaaaaa");
  let accessToken = localStorage.getItem("AccessToken");
  if (accessToken){
    return next(req.clone({
      headers: req.headers.set('Authorization',`Bearer ${accessToken}`)
    }));
  }
  return next(req);
};

export const IS_PUBLIC = new HttpContextToken(() => false);
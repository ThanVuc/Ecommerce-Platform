import { HttpContext, HttpContextToken, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { inject } from '@angular/core';
import { ɵparseCookieValue } from '@angular/common';

export const addJwtInterceptor: HttpInterceptorFn = (req, next) => {
  req = req.clone({
    withCredentials: true, // Ensure cookies are sent with every request
  });

  if (req.context.get(IS_PUBLIC)) {
    return next(req);
  }

  return next(req);
};

export const IS_PUBLIC = new HttpContextToken(() => false);
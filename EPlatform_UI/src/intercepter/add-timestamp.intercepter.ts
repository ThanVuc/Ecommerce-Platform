import { HttpContext, HttpContextToken, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { LocalStorageService } from '../app/components/services/local-storage.service';
import { inject } from '@angular/core';
import { ɵparseCookieValue } from '@angular/common';

export const addTimeStampIntercepter: HttpInterceptorFn = (req, next) => {
    req = req.clone({
        setHeaders: { 'Cache-Control': 'no-cache', 'Pragma': 'no-cache' },
        params: req.params.append('timestamp', Date.now().toString())
    });
    return next(req);
};

export const IS_PUBLIC = new HttpContextToken(() => false);
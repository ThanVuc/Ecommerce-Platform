import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, Title } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, provideHttpClient, withFetch, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { addJwtInterceptor } from '../guard/add-jwt.interceptor';
import { HandleErrService } from '../guard/handle-err.service';
import { addTimeStampIntercepter } from '../intercepter/add-timestamp.intercepter';

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(),
    provideHttpClient(
      withFetch(), 
      withInterceptors([addJwtInterceptor, addJwtInterceptor]),
      withInterceptorsFromDi()
    ),
    {provide: HTTP_INTERCEPTORS, useClass: HandleErrService, multi: true},
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), 
    provideClientHydration()]
};

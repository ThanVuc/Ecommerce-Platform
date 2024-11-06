import { Routes } from '@angular/router';
import path from 'path';
import { CustomerlayoutComponent } from './shares/layouts/customerlayout/customerlayout.component';
import { AdminlayoutComponent } from './shares/layouts/adminlayout/adminlayout.component';
import { AuthlayoutComponent } from './shares/layouts/authlayout/authlayout.component';
import { LoginComponent } from './components/auth/login/login.component';
import { register } from 'module';
import { RegisterComponent } from './components/auth/register/register.component';
import { ErrorlayoutComponent } from './shares/layouts/errorlayout/errorlayout.component';
import { ResetpasswordComponent } from './components/auth/resetpassword/resetpassword.component';
import { ForgetpasswordComponent } from './components/auth/forgetpassword/forgetpassword.component';
import { inject } from '@angular/core';
import { AuthGuard } from '../guard/auth.guard';
import { title } from 'process';

export const routes: Routes = [
    {
        path: '',
        loadChildren() {
            return import('./shares/layouts/layout.routes')
            .then(r => r.routes)
        }
    },
    {
        path: '',
        component: CustomerlayoutComponent
    },
    {
        path: 'admin',
        component: AdminlayoutComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'auth',
        component: AuthlayoutComponent,
        loadChildren: () => [
            {
                path: '',
                redirectTo: 'login',
                pathMatch: 'full'
            },
            {
                path: 'login',
                component: LoginComponent
            },
            {
                path: 'register',
                component: RegisterComponent
            },
            {
                path: 'reset-password',
                component: ResetpasswordComponent,
                canActivate: [AuthGuard]
            },
            {
                path: 'forget-password',
                component: ForgetpasswordComponent,
                canActivate: [AuthGuard]
            }
        ]
    },
    {
        path: '**',
        component: ErrorlayoutComponent
    }
];

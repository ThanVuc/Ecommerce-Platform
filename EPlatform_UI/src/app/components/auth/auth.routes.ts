import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent() {
            return import('./login/login.component')
            .then(lg => lg.LoginComponent)
        }
    },
    {
        path: '',
        loadComponent() {
            return import('./register/register.component')
            .then(rg => rg.RegisterComponent)
        },
    },
    {
        path: '',
        loadComponent() {
            return import('./resetpassword/resetpassword.component')
            .then(rp => rp.ResetpasswordComponent)
        },
    }
];
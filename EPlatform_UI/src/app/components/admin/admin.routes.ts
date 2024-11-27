import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent() {
            return import('./role/role.component')
            .then(r => r.RoleComponent)
        }
    },
    {
        path: '',
        loadComponent() {
            return import('./role-detail/role-detail.component')
            .then(rd => rd.RoleDetailComponent)
        },
    },
    {
        path: '',
        loadComponent() {
            return import('./users/user.component')
            .then(u => u.UserComponent)
        },
    },
    {
        path: '',
        loadComponent(){
            return import('./user-detail/user-detail.component')
            .then(ud => ud.UserDetailComponent)
        }
    }
];
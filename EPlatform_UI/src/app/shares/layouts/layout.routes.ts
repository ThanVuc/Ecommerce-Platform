import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent() {
            return import('./customerlayout/customerlayout.component')
            .then(cl => cl.CustomerlayoutComponent)
        }
    },
    {
        path: '',
        loadComponent() {
            return import('./authlayout/authlayout.component')
            .then(al => al.AuthlayoutComponent)
        }
    },
    {
        path: '',
        loadComponent() {
            return import('./adminlayout/adminlayout.component')
            .then(adl => adl.AdminlayoutComponent)
        }
    },
    {
        path: '',
        loadComponent() {
            return import('./shopownerlayout/shopownerlayout.component')
            .then(sl => sl.ShopownerlayoutComponent)
        },
    },
    {
        path: '',
        loadComponent() {
            return import('../reusable/not-found-page/not-found-page.component')
            .then(err => err.NotFoundPageComponent)
        },
    }
];
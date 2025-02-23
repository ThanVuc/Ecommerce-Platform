import { Routes } from '@angular/router';
import path from 'path';
import { CustomerlayoutComponent } from './shares/layouts/customerlayout/customerlayout.component';
import { AdminlayoutComponent } from './shares/layouts/adminlayout/adminlayout.component';
import { AuthlayoutComponent } from './shares/layouts/authlayout/authlayout.component';
import { LoginComponent } from './components/auth/login/login.component';
import { register } from 'module';
import { RegisterComponent } from './components/auth/register/register.component';
import { ResetpasswordComponent } from './components/auth/resetpassword/resetpassword.component';
import { inject } from '@angular/core';
import { AuthGuard } from '../guard/auth.guard';
import { title } from 'process';
import { HomeCustomerComponent } from './components/customer/home-customer/home-customer.component';
import { ShopownerlayoutComponent } from './shares/layouts/shopownerlayout/shopownerlayout.component';
import { RoleComponent } from './components/admin/role/role.component';
import { RoleDetailComponent } from './components/admin/role-detail/role-detail.component';
import { UserComponent } from './components/admin/users/user.component';
import { UserDetailComponent } from './components/admin/user-detail/user-detail.component';
import { NotFoundError } from 'rxjs';
import { NotFoundPageComponent } from './shares/reusable/not-found-page/not-found-page.component';
import { ProductsComponent } from './components/shopowner/products/products.component';
import { AddProductComponent } from './components/shopowner/add-product/add-product.component';
import { CreateShopComponent } from './components/shopowner/create-shop/create-shop.component';

export const routes: Routes = [
    {
        path: '',
        component: CustomerlayoutComponent,
        loadChildren: () => [
            {
                path: '',
                redirectTo: 'home',
                pathMatch: 'full'
            },
            {
                path: 'home',
                component: HomeCustomerComponent
            }
        ]
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
            }
        ]
    },
    {
        path: 'admin',
        component: AdminlayoutComponent,
        canActivate: [AuthGuard],
        loadChildren: () => [
            {
                path: '',
                redirectTo: 'roles',
                pathMatch: 'full'
            },
            {
                path: 'roles',
                component: RoleComponent
            },
            {
                path: 'roles/:id',
                component: RoleDetailComponent
            },
            {
                path: 'users',
                component: UserComponent
            },
            {
                path: 'users/:id',
                component: UserDetailComponent
            }
        ]
    },
    {
        path: "shop-owner/create-shop",
        component: CreateShopComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'shop-owner/:shop_id',
        component: ShopownerlayoutComponent,
        loadChildren: () => [
            {
                path: "products",
                component: ProductsComponent
            },
            {
                path: "add-product",
                component: AddProductComponent
            },
            {
                path: 'products/:product_id/update',
                component: AddProductComponent
            },
            {
                path: 'products/:product_id',
                component: AddProductComponent
            }
        ],
    },
    {
        path: '**',
        component: NotFoundPageComponent
    }
];

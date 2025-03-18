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
import { ProductDetailComponent } from './components/customer/product-detail/product-detail.component';
import { CartsComponent } from './components/customer/carts/carts.component';
import { OrdersComponent } from './components/shopowner/orders/orders.component';

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
                loadComponent() {
                    return import('./components/customer/home-customer/home-customer.component')
                    .then(hc => hc.HomeCustomerComponent)
                },
            },
            {
                path: 'products/:product_slug',
                loadComponent() {
                    return import('./components/customer/product-detail/product-detail.component')
                    .then(pd => pd.ProductDetailComponent)
                },
            },
            {
                path: 'carts',
                loadComponent() {
                    return import('./components/customer/carts/carts.component')
                    .then(c => c.CartsComponent)
                },
                canActivate: [AuthGuard]
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
                loadComponent() {
                    return import('./components/auth/login/login.component')
                    .then(l => l.LoginComponent)
                },
            },
            {
                path: 'register',
                loadComponent() {
                    return import('./components/auth/register/register.component')
                    .then(r => r.RegisterComponent)
                },
            },
            {
                path: 'reset-password',
                loadComponent() {
                    return import('./components/auth/resetpassword/resetpassword.component')
                    .then(rp => rp.ResetpasswordComponent)
                },
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
                loadComponent() {
                    return import('./components/admin/role/role.component')
                    .then(r => r.RoleComponent)
                },
            },
            {
                path: 'roles/:id',
                loadComponent() {
                    return import('./components/admin/role-detail/role-detail.component')
                    .then(rd => rd.RoleDetailComponent)
                },
            },
            {
                path: 'users',
                loadComponent() {
                    return import('./components/admin/users/user.component')
                    .then(u => u.UserComponent)
                },
            },
            {
                path: 'users/:id',
                loadComponent() {
                    return import('./components/admin/user-detail/user-detail.component')
                    .then(ud => ud.UserDetailComponent)
                },
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
        canActivate: [AuthGuard],
        loadChildren: () => [
            {
                path: "products",
                loadComponent(){
                    return import('./components/shopowner/products/products.component')
                    .then(p => p.ProductsComponent)
                }
            },
            {
                path: "add-product",
                loadComponent() {
                    return import('./components/shopowner/add-product/add-product.component')
                    .then(ap => ap.AddProductComponent)
                },
            },
            {
                path: 'products/:product_id',
                loadComponent() {
                    return import('./components/shopowner/add-product/add-product.component')
                    .then(ap => ap.AddProductComponent)
                },
            },
            {
                path: 'products/:product_id/update',
                loadComponent() {
                    return import('./components/shopowner/add-product/add-product.component')
                    .then(ap => ap.AddProductComponent)
                },
            },
            {
                path: 'orders',
                loadComponent() {
                    return import('./components/shopowner/orders/orders.component')
                    .then(o => o.OrdersComponent)
                },
                loadChildren: () => [
                    {
                        path: '',
                        loadComponent() {
                            return import('./components/shopowner/orders/orders-management/orders-management.component')
                            .then(o => o.OrdersManagementComponent)
                        }
                    },
                    {
                        path: ':order_id',
                        loadComponent() {
                            return import('./components/shopowner/orders/order-detail/order-detail.component')
                            .then(od => od.OrderDetailComponent)
                        }
                    }
                ]
            }
        ],
    },
    {
        path: '**',
        component: NotFoundPageComponent
    }
];

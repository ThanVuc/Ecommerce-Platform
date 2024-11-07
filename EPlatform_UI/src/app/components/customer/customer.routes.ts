import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent() {
            return import('./home-customer/home-customer.component')
            .then(h => h.HomeCustomerComponent);
        }
    }
];
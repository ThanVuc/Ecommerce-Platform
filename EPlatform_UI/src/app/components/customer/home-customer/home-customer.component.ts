import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ProductService } from '../../services/product.service';
import { RootCategoryModel } from '../models/root-category-model';
import { ProductBriefModel } from '../models/product-brief-model';
import { RouterLink } from '@angular/router';
import { SignalRService } from '../../services/signal-r.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-home-customer',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home-customer.component.html',
  styleUrl: './home-customer.component.scss'
})

export class HomeCustomerComponent implements OnInit {
  async ngOnInit(): Promise<void> {
    console.log('ngOnInit called');
    try {
      console.log("Trigger ng onInit in HomeCustomerComponent");
      await this.isAuthenticated();
    } finally {
      console.log('Calling methods after authentication');
      this.getCategoriesInHome();
      this.getHotProduct();
      this.getProductTodaySuggestions();
    }
  }

  productSVC = inject(ProductService);
  authSVC = inject(AuthService);
  categories: CategoriesHandleModel = {
    start: 0,
    showCount: 6,
    categoiesInHome: []
  };

  productHandler: ProductHandleModel = {
    start: 0,
    showCount: 5,
    products: []
  };

  productTodaySuggestions: ProductBriefModel[] = [];
  isAuthenticatedState: boolean = false; // New property to track authentication state

  async isAuthenticated(): Promise<void> {
    try {
      await firstValueFrom(this.authSVC.IsAuthenticatedOrRefresh());
      this.isAuthenticatedState = true;
    } catch (error: any) {
      this.isAuthenticatedState = false; // Update state on failure
    }
  }

  getCategoriesInHome() {
    this.productSVC.getCategoriesInHome().subscribe({
      next: res => {
        this.categories.categoiesInHome = res.data;
      },
      error: err => {
        console.log(err);
      }
    });
  }

  getHotProduct() {
    console.log('Fetching hot products...');
    this.productSVC.getHotProducts().subscribe({
      next: res => {
        console.log('Hot products fetched:', res.data);
        this.productHandler.products = res.data;
      },
      error: err => {
        console.error('Error fetching categories:', err);
      }
    });
  }

  categoryChange(isPrev: boolean) {
    if (isPrev) {
      this.categories.start = this.categories.start - this.categories.showCount;
    } else {
      this.categories.start = this.categories.start + this.categories.showCount > this.categories.categoiesInHome.length ? this.categories.start : this.categories.start + this.categories.showCount;
    }
  }

  productChange(isPrev: boolean) {
    if (isPrev) {
      this.productHandler.start = this.productHandler.start - this.productHandler.showCount;
    } else {
      this.productHandler.start = this.productHandler.start + this.productHandler.showCount > this.productHandler.products.length ? this.productHandler.start : this.productHandler.start + this.productHandler.showCount;
    }
  }

  getProductTodaySuggestions() {
    console.log("getProductTodaySuggestions");
    this.productSVC.getProductTodaySuggestions().subscribe({
      next: res => {
        this.productTodaySuggestions = res.data;
      },
      error: err => {
        console.log(err);
      }
    });
  }


}

export interface CategoriesHandleModel {
  start: number;
  showCount: number;
  categoiesInHome: RootCategoryModel[];
}

export interface ProductHandleModel {
  start: number;
  showCount: number;
  products: ProductBriefModel[];
}
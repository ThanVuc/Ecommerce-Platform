import { Component, inject, OnInit } from '@angular/core';
import { ProductDetailModel } from '../models/product-detail-model';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AddToCartModel } from '../models/add-to-cart-model';
import { spec } from 'node:test/reporters';
import { AuthService } from '../../services/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  async ngOnInit(): Promise<void> {
    await this.isAuthenticated();
    const productId = this.activatedRoute.snapshot.queryParamMap.get("product_id");
    if (productId) {
      this.productId = parseInt(productId);
      this.addToCartModel.productId = this.productId;
      this.productSVC.getProductDetail(this.productId).subscribe({
        next: res => {
          this.productDetail = res.data;
          this.availabel = this.productDetail.availabel;
          this.images.push(this.productDetail.avtImageUrl);
          this.productDetail.specAttributes[0].specItems.forEach((specItem) => {
            this.images.push(specItem.specImageUrl);
          });

          if (this.productDetail.specAttributes.length > 1) {
            this.spec.primaryName = this.productDetail.specAttributes[0].specName;
            this.spec.secondaryName = this.productDetail.specAttributes[1].specName;
          } else {
            this.spec.primaryName = this.productDetail.specAttributes[0].specName;
            this.spec.secondaryName = '';
          }

          if (this.images.length > 4){
            this.imagesIndex = [0, 1, 2, 3];
          } else {
            this.imagesIndex = Array.from({length: this.images.length}, (_, i) => i);
          }
        },
        error: err => {
          this.router.navigateByUrl('/not-found');
          console.log(err);
        }
      });
    }
  }

  activatedRoute = inject(ActivatedRoute);
  productSVC = inject(ProductService);
  router = inject(Router);
  authSVC = inject(AuthService);

  productDetail: ProductDetailModel = {
    name: '',
    price: 0,
    description: '',
    avtImageUrl: '',
    sold: '0',
    availabel: 0,
    specAttributes: [],
    specInventories: [],
    categories: [],
    shopId: '',
    shopName: '',
    logoUrl: ''
  };
  productId = 0;
  availabel = 0;
  spec = {
    primaryName: '',
    primary: '',
    secondaryName: '',
    secondary: ''
  };
  images: string[] = [];
  imagesIndex: number[] = [];
  currentImageIndex = 0;
  addToCartModel: AddToCartModel = {
    quantity: 1,
    specInfo: '',
    productId: 0
  };
  isAuthenticatedState = false;

  async isAuthenticated(): Promise<void> {
      try {
        await firstValueFrom(this.authSVC.IsAuthenticatedOrRefresh());
        this.isAuthenticatedState = true;
      } catch (error: any) {
        if (error.status == 401) {
          this.isAuthenticatedState = false; // Update state on failure
        } else {
          console.error("Error checking authentication: ", error);
        }
      }
    }

  minusQuantity() {
    if (this.addToCartModel.quantity > 1) {
      this.addToCartModel.quantity--;
    }
  }

  plusQuantity() {
    if (this.addToCartModel.quantity < this.availabel) {
      this.addToCartModel.quantity++;
    }
  }

  chooseSpecValue(event: Event, specName: string, isPrimary: boolean = true, imgSrc: string = '') {
    const target = event.currentTarget as HTMLElement;
    const parent = target.parentElement;

    if (target.classList.contains('active')) {
      target.classList.remove('active');
      if (isPrimary) {
        this.spec.primary = '';
        document.querySelectorAll('.spec-value').forEach((element) => {
          element.classList.remove('active');
        });
      }
      this.spec.secondary = '';
      return;
    }

    if (isPrimary) {
      this.spec.primary = specName;
      const mainImage = document.getElementById('main-image') as HTMLImageElement;
      if (mainImage) {
        mainImage.src = imgSrc;
      }
    } else {
      if (this.spec.primary === '') {
        const primaryParent = document.getElementById('spec-primary-value');

        if (primaryParent) {
          primaryParent.querySelectorAll('.spec-value').forEach((element) => {
            element.classList.add('invalid');
            setTimeout(() => {
              element.classList.remove('invalid');
            }, 300);
          });
        }

        return;
      }
      this.spec.secondary = specName;
    }

    if (parent) {
      parent.querySelectorAll('.spec-value').forEach((element) => {
        element.classList.remove('active');
      });
    }

    target.classList.add('active');
    this.setInventory();
  }

  setInventory(){
    let availabel = 0;
    if (this.spec.primary){
      if (this.spec.secondary) {
        // get spec with specific primary and secondary
        for (const spec of this.productDetail.specInventories) {
          if (spec.primarySpecValueName === this.spec.primary && spec.subSpecValueName === this.spec.secondary){
            availabel = spec.inventory;
            break;
          }
        }
      } else {
        for (const spec of this.productDetail.specInventories) {
          if (spec.primarySpecValueName === this.spec.primary){
            availabel += spec.inventory;
          }
        }
      }
    }
    this.availabel = availabel;
  }

  hoverImage(idx: number) {
    this.currentImageIndex = idx;
  }

  prevImage() {
    if (this.imagesIndex[0] === 0) {
      return;
    }
    this.imagesIndex = this.imagesIndex.map((idx) => idx - 1);
  }

  nextImage() {
    if (this.imagesIndex[this.imagesIndex.length - 1] === this.images.length - 1) {
      return;
    }
    this.imagesIndex = this.imagesIndex.map((idx) => idx + 1);
  }

  addToCart(){
    if (!this.checkProductConditionToAddToCart()){
      return;
    }

    this.productSVC.addToCart(this.addToCartModel).subscribe({
      next: res => {
        alert("Add to cart successfully");
      },
      error: err => {
        console.log(err);
      }
    });
  }

  buyNow(){
    // add to cart
    // go to cart page
    // select this product -> assign isChecked = true
    // wanna isChecked = true -> get cart item id
    // get from add to cart method -> desgin backend 
    if (!this.checkProductConditionToAddToCart()){
      return;
    }

    this.productSVC.addToCart(this.addToCartModel).subscribe({
      next: res => {
        const cartItemId = res.data;
        this.router.navigateByUrl('/carts?cart_item_id=' + cartItemId);
      },
      error: err => {
        console.log(err);
      }
    });
  }

  checkProductConditionToAddToCart() : boolean{
    if (!this.isAuthenticatedState){
      this.router.navigateByUrl('/auth/login');
      return false;
    }

    if (this.spec.primaryName === ''){
      alert('This product is invalid');
      return false;
    }

    if (this.spec.primaryName && !this.spec.primary || this.spec.secondaryName && !this.spec.secondary){
      alert('Please choose the classify options');
      return false;
    } else {
      this.addToCartModel.specInfo = `${this.spec.primaryName}: ${this.spec.primary}`;
      if (this.spec.secondary){
        this.addToCartModel.specInfo += `, ${this.spec.secondaryName}: ${this.spec.secondary}`;
      }
    }

    if (document.querySelectorAll('.err').length > 0){
      alert("please, fix all mistakes");
      return false;
    }

    return true;
  }
}

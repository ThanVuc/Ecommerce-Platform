import { Component, inject, OnInit } from '@angular/core';
import { ProductDetailModel } from '../models/product-detail-model';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  ngOnInit(): void {
    const productId = this.activatedRoute.snapshot.queryParamMap.get("product_id");
    if (productId) {
      this.productId = parseInt(productId);
      this.productSVC.getProductDetail(this.productId).subscribe({
        next: res => {
          this.productDetail = res.data;
          this.availabel = this.productDetail.availabel;
          this.images.push(this.productDetail.avtImageUrl);
          this.productDetail.specAttributes[0].specItems.forEach((specItem) => {
            this.images.push(specItem.specImageUrl);
          });
          if (this.images.length > 4){
            this.imagesIndex = [0, 1, 2, 3];
          } else {
            this.imagesIndex = Array.from({length: this.images.length}, (_, i) => i);
          }
        },
        error: err => {
          console.log(err);
        }
      });
    }
  }

  activatedRoute = inject(ActivatedRoute);
  productSVC = inject(ProductService);
  productDetail: ProductDetailModel = {
    name: 'Product Name',
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
  buyQuantity = 1;
  spec = {
    primary: '',
    secondary: ''
  };
  images: string[] = [];
  imagesIndex: number[] = [];
  currentImageIndex = 0;

  minusQuantity() {
    if (this.buyQuantity > 1) {
      this.buyQuantity--;
    }
  }

  plusQuantity() {
    if (this.buyQuantity < this.availabel) {
      this.buyQuantity++;
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
}

import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ProductPostModel, SpecAttribute, WarehouseItem } from '../models/product-post-model';
import { FormsModule } from '@angular/forms';
import { HtmlParser } from '@angular/compiler';
import { UpperCasePipe } from '@angular/common';
import e from 'express';
import { SelectCategoryComponent } from "./select-category/select-category.component";
import { CategoryModel } from '../models/category-model';
import { UploadImagesComponent } from "./upload-images/upload-images.component";
import { SelectTagComponent } from "../../../shares/reusable/select-tag/select-tag.component";
import { spec } from 'node:test/reporters';
import { selectModel } from '../../../shares/reusable/common-model/select-model';
import { UtilitiesServiceService } from '../../services/utilities-service.service';
import { ShopService } from '../../services/shop.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [FormsModule, SelectCategoryComponent, UploadImagesComponent, SelectTagComponent],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.scss'
})

export class AddProductComponent implements OnInit {
  @ViewChild(UploadImagesComponent) uploadImages!: UploadImagesComponent;

  ngOnInit(): void {
    this.getWarehousesForSelect();
    this.activatedRoute.parent?.params.subscribe(params => { 
      this.shopId = params['shop_id'];
    });
  }

  warehouseItems: WarehouseItem[] = [{
    id: '1',
    name: "Warehouse 1"
  }, {
    id: '2',
    name: "Warehouse 2"
  }];

  isSingleSpec: boolean = true;

  category: CategoryModel = {
    categoryId: null,
    name: "temp",
    isNext: false
  };
  productModel: ProductPostModel = {
    Name: '',
    Description: '',
    Price: 0,
    CategoryId: 0,
    IsPublic: true,
    SpecAttributes: [],
    SpecInventories: [],
    WarehouseId: 0,
    TotalInventory: 0,
    CoverImage: null
  }
  shopId: string = '';

  utilitiesSVC = inject(UtilitiesServiceService);
  shopSVC = inject(ShopService);
  activatedRoute = inject(ActivatedRoute);

  getWarehousesForSelect() {
    this.utilitiesSVC.getWarehouses().subscribe({
      next: (res) => {
        this.warehouseItems = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  setPublic(isPublic: boolean) {
    this.productModel.IsPublic = isPublic;
  }

  saveCategory(category: CategoryModel) {
    this.category = category;
    if (category.categoryId){
      this.productModel.CategoryId = category.categoryId;
    }
  }

  findCurrentSpec(event: Event) {
    const targetElement = event.target as HTMLElement;
    const parentElement = targetElement.parentElement;
    const specNameElement = parentElement?.querySelector('.name') as HTMLInputElement;

    if (this.productModel.SpecAttributes.find(spec => spec.SpecName === specNameElement.value) === undefined) {
      return '';
    }

    return specNameElement.value;
  }

  triggerError(inputElement: HTMLInputElement) {
    inputElement.classList.add("trigger-error");
    inputElement.addEventListener("animationend", () => {
      inputElement.classList.remove("trigger-error");
    }, { once: true });
  }

  saveSpec(event: Event) {
    event.preventDefault();
    const valueInputElement = event.target as HTMLInputElement;
    const currentSpec = this.findCurrentSpec(event);
    let specItems = this.productModel.SpecAttributes
      .find(spec => spec.SpecName === currentSpec)
      ?.SpecItems;

    if (!specItems) {
      this.triggerError(valueInputElement);
      return;
    }
    let currentItem = specItems.find(specItem => specItem.SpecValue === valueInputElement.value);

    if (currentItem) {
      this.triggerError(valueInputElement);
      return;
    }

    specItems.push({
      SpecValue: valueInputElement.value,
      SpecImage: null
    });

    switch (this.productModel.SpecAttributes.length) {
      case 1:
        this.productModel.SpecInventories.push({
          PrimarySpecValueName: valueInputElement.value,
          SubSpecValueName: '',
          Inventory: 0,
        });
        break;
      case 2:
        if (this.isSingleSpec){
          this.isSingleSpec = false;
          this.productModel.SpecInventories = [];
        }
        this.productModel.SpecAttributes[0].SpecItems.forEach(specItem => {
          this.productModel.SpecInventories.push({
            PrimarySpecValueName: specItem.SpecValue,
            SubSpecValueName: valueInputElement.value,
            Inventory: 0
          });
        });

        break;
      default:
        break      
    }
    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    const valueElement = productSpecElement?.querySelector(".value") as HTMLInputElement;
    valueElement?.classList.toggle("hide");
    valueElement.value = '';
    productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");

  }

  addSpec(event: Event) {
    if (this.productModel.SpecAttributes.find(spec => spec.SpecName === '') !== undefined) {
      alert("Can't add empty spec continiously");
      const targetElement = event.target as HTMLInputElement;
      this.triggerError(targetElement);
      return;
    }

    let isPrimary: boolean = false;

    if (this.productModel.SpecAttributes.length === 0) {
      isPrimary = true;
    }

    this.productModel.SpecAttributes.push({
      SpecName: '',
      SpecItems: [],
      IsPrimary: isPrimary
    });
  }

  saveSpecName(event: Event, name: string) {
    event.preventDefault();
    const targetElement = event.target as HTMLInputElement;
    let specObject = this.productModel.SpecAttributes.find(spec => spec.SpecName === name);
    if (specObject) {
      specObject.SpecName = targetElement.value;
    }
    targetElement.blur();
  }

  removeSpecValue(name: string, value: string) {
    let specItems = this.productModel.SpecAttributes.find(spec => spec.SpecName === name)?.SpecItems;
    //delete the spec inventory for me by condition: spec.PrimarySpecValueName === value || spec.SubSpecValueName === value
    this.productModel.SpecInventories = this.productModel.SpecInventories.filter(spec => 
      spec.PrimarySpecValueName !== value && spec.SubSpecValueName !== value
    );

    
    if (specItems) {
      for (let i = 0; i < specItems.length; i++) {
        if (specItems[i].SpecValue === value) {
          specItems.splice(i, 1);
          break;
        }
      }
    }
  }

  removeSpec(specName: string) {
    const specIndex = this.productModel.SpecAttributes.findIndex(spec => spec.SpecName === specName);
    if (specIndex !== -1) {
      this.productModel.SpecAttributes.splice(specIndex, 1);
      if (this.productModel.SpecAttributes.length == 1) {
        this.productModel.SpecAttributes[0].IsPrimary = true;
      }
      this.uploadImages.productModel = this.productModel;
    }
    console.log(this.productModel.SpecAttributes);
  }

  showSpecValue(event: Event) {
    event.preventDefault();
    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    productSpecElement?.querySelector(".value")?.classList.toggle("hide");
    productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");
  }

  getWarehouse(warehouseItem: WarehouseItem) {
    this.productModel.WarehouseId = parseInt(warehouseItem.id);
  }

  saveUploadImagesAndInventories(productModel: ProductPostModel) {
    this.productModel = productModel;
    let sum:number = 0;
    for(let i = 0; i < this.productModel.SpecInventories.length; i++){
      sum += this.productModel.SpecInventories[i].Inventory;
    }
    this.productModel.TotalInventory = sum;
  }

  createNewProduct(){
    this.shopSVC.addProduct(this.shopId,this.productModel).subscribe({
      next: (res) => {
        console.log(res.data);
      },
      error: (err) => {
        console.log(err);
      }
    });
  }
}

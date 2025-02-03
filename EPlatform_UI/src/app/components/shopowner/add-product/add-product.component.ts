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
    name: '',
    description: '',
    price: 0,
    categoryId: 0,
    isPublic: true,
    specAttributes: [],
    specInventories: [],
    warehouseId: 0,
    totalInventory: 0
  }

  utilitiesSVC = inject(UtilitiesServiceService);

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
    this.productModel.isPublic = isPublic;
  }

  saveCategory(category: CategoryModel) {
    this.category = category;
  }

  findCurrentSpec(event: Event) {
    const targetElement = event.target as HTMLElement;
    const parentElement = targetElement.parentElement;
    const specNameElement = parentElement?.querySelector('.name') as HTMLInputElement;

    if (this.productModel.specAttributes.find(spec => spec.specName === specNameElement.value) === undefined) {
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
    let specItems = this.productModel.specAttributes
      .find(spec => spec.specName === currentSpec)
      ?.specItems;

    if (!specItems) {
      this.triggerError(valueInputElement);
      return;
    }
    let currentItem = specItems.find(specItem => specItem.specValue === valueInputElement.value);

    if (currentItem) {
      this.triggerError(valueInputElement);
      return;
    }

    specItems.push({
      specValue: valueInputElement.value,
      specImage: null
    });

    switch (this.productModel.specAttributes.length) {
      case 1:
        this.productModel.specInventories.push({
          primarySpecValueName: valueInputElement.value,
          subSpecValueName: '',
          inventory: 0,
        });
        break;
      case 2:
        if (this.isSingleSpec){
          this.isSingleSpec = false;
          this.productModel.specInventories = [];
        }
        this.productModel.specAttributes[0].specItems.forEach(specItem => {
          this.productModel.specInventories.push({
            primarySpecValueName: specItem.specValue,
            subSpecValueName: valueInputElement.value,
            inventory: 0
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
    if (this.productModel.specAttributes.find(spec => spec.specName === '') !== undefined) {
      alert("Can't add empty spec continiously");
      const targetElement = event.target as HTMLInputElement;
      this.triggerError(targetElement);
      return;
    }

    let isPrimary: boolean = false;

    if (this.productModel.specAttributes.length === 0) {
      isPrimary = true;
    }

    this.productModel.specAttributes.push({
      specName: '',
      specItems: [],
      isPrimary: isPrimary
    });
  }

  saveSpecName(event: Event, name: string) {
    event.preventDefault();
    const targetElement = event.target as HTMLInputElement;
    let specObject = this.productModel.specAttributes.find(spec => spec.specName === name);
    if (specObject) {
      specObject.specName = targetElement.value;
    }
    targetElement.blur();
  }

  removeSpecValue(name: string, value: string) {
    let specItems = this.productModel.specAttributes.find(spec => spec.specName === name)?.specItems;

    if (specItems) {
      for (let i = 0; i < specItems.length; i++) {
        if (specItems[i].specValue === value) {
          specItems.splice(i, 1);
          break;
        }
      }
    }

  }

  removeSpec(specName: string) {
    const specIndex = this.productModel.specAttributes.findIndex(spec => spec.specName === specName);
    if (specIndex !== -1) {
      this.productModel.specAttributes.splice(specIndex, 1);
      if (this.productModel.specAttributes.length == 1) {
        this.productModel.specAttributes[0].isPrimary = true;
      }
      this.uploadImages.productModel = this.productModel;
    }
    console.log(this.productModel.specAttributes);
  }

  showSpecValue(event: Event) {
    event.preventDefault();
    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    productSpecElement?.querySelector(".value")?.classList.toggle("hide");
    productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");
  }

  getWarehouse(warehouseItem: WarehouseItem) {
    this.productModel.warehouseId = parseInt(warehouseItem.id);
  }

  saveUploadImagesAndInventories(productModel: ProductPostModel) {
    this.productModel = productModel;
    let sum:number = 0;
    for(let i = 0; i < this.productModel.specInventories.length; i++){
      sum += this.productModel.specInventories[i].inventory;
    }
    this.productModel.totalInventory = sum;
    console.log(this.productModel.totalInventory);
  }
}

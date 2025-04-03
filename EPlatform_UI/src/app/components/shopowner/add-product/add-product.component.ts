import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ProductPostModel, SpecAttribute, WarehouseItem } from '../models/product-post-model';
import { FormsModule } from '@angular/forms';
import { Expansion, HtmlParser } from '@angular/compiler';
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
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageComponent } from "../../../shares/reusable/message/message.component";
import { Title } from '@angular/platform-browser';
import { ProductCreateUpdateModel } from '../models/product-create-update-model';
import { AdminService } from '../../services/admin.service';
import { error } from 'console';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [FormsModule,RouterLink, SelectCategoryComponent, UploadImagesComponent, SelectTagComponent, MessageComponent],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.scss'
})

export class AddProductComponent implements OnInit {
  @ViewChild(UploadImagesComponent) uploadImages!: UploadImagesComponent;
  @ViewChild(MessageComponent) messager!: MessageComponent;
  @ViewChild(SelectTagComponent) selectTag!: SelectTagComponent;
  @ViewChild(SelectCategoryComponent) selectCategory!: SelectCategoryComponent;

  ngOnInit(): void {
    this.getWarehousesForSelect();
    this.activatedRoute.parent?.params.subscribe(params => {
      this.shopId = params['shop_id'];
    });

    this.activatedRoute.params.subscribe(params => {
      this.productId = params['product_id'];
      if (this.productId) {
        this.isDetail = this.router.url.split('/').at(-1) === 'update' ? false : true;
        if (this.isDetail){
          this.handleDetailProduct();
        } else {
          this.handleUpdateProduct();
        }
      } else {
        this.titleSVC.setTitle("Product Add");
      }
    });
  }

  titleSVC: Title = inject(Title);
  warehouseItems: WarehouseItem[] = [];
  isSingleSpec: boolean = true;

  category: CategoryModel = {
    categoryId: null,
    name: "temp",
    isNext: false
  };
  productModel: ProductCreateUpdateModel = {
    Name: '',
    Description: '',
    Price: 0,
    CategoryId: 0,
    IsPublic: true,
    SpecAttributes: [],
    SpecInventories: [],
    WarehouseId: 0,
    TotalInventory: 0,
    CoverImage: null,
    CoverImageUrl: null,
    Slug: null,
    CreatedAt: '',
    UpdatedAt: ''
  }
  shopId: string = '';

  utilitiesSVC = inject(UtilitiesServiceService);
  shopSVC = inject(ShopService);
  activatedRoute = inject(ActivatedRoute);
  router = inject(Router);
  productId: string | null = null;
  isDetail: boolean = false;

  handleUpdateProduct(){
    if (this.productId == null){
      throw new Error("Product isn't updating status");
    }
    this.titleSVC.setTitle("Product Update");
    this.shopSVC.getUpdateProduct(this.productId).subscribe({
      next: (res) => {
        this.productModel = {
          Name: res.data.name,
          Description: res.data.description,
          Price: res.data.price,
          CategoryId: res.data.categoryId,
          IsPublic: res.data.isPublic,
          SpecAttributes: res.data.specAttributes.map((spec) => {
            return {
              SpecName: spec.specName,
              IsPrimary: spec.isPrimary,
              SpecItems: spec.specItems.map((specItem) => {
                return {
                  SpecValue: specItem.specValue,
                  SpecImage: null,
                  SpecImageUrl: specItem.specImageUrl
                }
              })
            }
          }),
          SpecInventories: res.data.specInventories.map((specInventory) => {
            return {
              PrimarySpecValueName: specInventory.primarySpecValueName,
              SubSpecValueName: specInventory.subSpecValueName,
              Inventory: specInventory.inventory
            }
          }),
          WarehouseId: res.data.warehouseId,
          TotalInventory: res.data.totalInventory,
          CoverImage: null,
          CoverImageUrl: res.data.coverImageUrl,
          Slug: res.data.slug,
          CreatedAt: '',
          UpdatedAt: ''
        }
        this.selectCategory.setCategory({
          categoryId: res.data.categoryId,
          name: res.data.categoryName,
          isNext: false
        });
        this.selectTag.setValueFromParent(res.data.warehouseId.toString());
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  handleDetailProduct(){
    if (this.productId == null){
      throw new Error("Product isn't detail status");
    }
    this.titleSVC.setTitle("Product Detail");
    this.shopSVC.getDetailProduct(this.productId).subscribe({
      next: (res) => {
        this.productModel = {
          Name: res.data.name,
          Description: res.data.description,
          Price: res.data.price,
          CategoryId: res.data.categoryId,
          IsPublic: res.data.isPublic,
          SpecAttributes: res.data.specAttributes.map((spec) => {
            return {
              SpecName: spec.specName,
              IsPrimary: spec.isPrimary,
              SpecItems: spec.specItems.map((specItem) => {
                return {
                  SpecValue: specItem.specValue,
                  SpecImage: null,
                  SpecImageUrl: specItem.specImageUrl
                }
              })
            }
          }),
          SpecInventories: res.data.specInventories.map((specInventory) => {
            return {
              PrimarySpecValueName: specInventory.primarySpecValueName,
              SubSpecValueName: specInventory.subSpecValueName,
              Inventory: specInventory.inventory
            }
          }),
          WarehouseId: res.data.warehouseId,
          TotalInventory: res.data.totalInventory,
          CoverImage: null,
          CoverImageUrl: res.data.coverImageUrl,
          Slug: res.data.slug,
          CreatedAt: new Date(res.data.createdAt).toLocaleString(),
          UpdatedAt: new Date(res.data.updatedAt).toLocaleString()
        }
        this.selectCategory.setCategory({
          categoryId: res.data.categoryId,
          name: res.data.categoryName,
          isNext: false
        });
        this.selectTag.setValueFromParent(res.data.warehouseId.toString());
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  async getWarehousesForSelect() {
    this.warehouseItems = (await this.utilitiesSVC.getWarehouses()).data;
  }

  setPublic(isPublic: boolean) {
    this.productModel.IsPublic = isPublic;
  }

  saveCategory(category: CategoryModel) {
    this.category = category;
    if (category.categoryId) {
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
    const currentSpecName = this.findCurrentSpec(event);
    let specAttribute = this.productModel.SpecAttributes
      .find(spec => spec.SpecName === currentSpecName);

    if (!specAttribute) {
      this.triggerError(valueInputElement);
      return;
    }

    let specItems = specAttribute.SpecItems;

    if (!specItems) {
      this.triggerError(valueInputElement);
      return;
    }
    let currentItem = specItems.find(specItem => specItem.SpecValue === valueInputElement.value);

    if (currentItem) {
      this.triggerError(valueInputElement);
      return;
    }

    const specValue = valueInputElement.value
    specItems.push({
      SpecValue: specValue,
      SpecImage: null,
      SpecImageUrl: null
    });


    let hasSubSpec = this.productModel.SpecAttributes.length > 1;
    let isPrimary = specAttribute.IsPrimary;
    console.log("primary: " + isPrimary);
    console.log("subspec: " + hasSubSpec);
    switch (isPrimary){
      case true:
        this.productModel.SpecInventories.push({
          PrimarySpecValueName: specValue,
          SubSpecValueName: '',
          Inventory: 0
        });
        if (hasSubSpec) {
          this.productModel.SpecAttributes[1].SpecItems.forEach(subSpec => {
            this.productModel.SpecInventories.push({
              PrimarySpecValueName: specValue,
              SubSpecValueName: subSpec.SpecValue,
              Inventory: 0
            });
          });
        }
        break;
      case false:
        this.productModel.SpecAttributes[0].SpecItems.forEach(primarySpec => {
          this.productModel.SpecInventories.push({
            PrimarySpecValueName: primarySpec.SpecValue,
            SubSpecValueName: specValue,
            Inventory: 0
          });
        });
        break;
      default:
        this.triggerError(valueInputElement);
        break;
    }

    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    const valueElement = productSpecElement?.querySelector(".value") as HTMLInputElement;
    // valueElement?.classList.toggle("hide");
    valueElement.value = '';
    // productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");
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
    console.log(this.productModel);

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

  saveUploadImagesAndInventories(productModel: ProductCreateUpdateModel) {
    this.productModel = productModel;
    let sum: number = 0;
    for (let i = 0; i < this.productModel.SpecInventories.length; i++) {
      sum += this.productModel.SpecInventories[i].Inventory;
    }
    this.productModel.TotalInventory = sum;
  }

  createNewProduct() {
    const errors = document.querySelectorAll(".extra-err");
    if (errors.length > 0) {
      this.messager.showModal("fail", "Please check again all the fields");
      errors.forEach(err => {
        err.classList.add("show");
      });
      return;
    }
    this.shopSVC.addProduct(this.shopId, this.productModel).subscribe({
      next: (res) => {
        this.messager.showModal("success", "Create Product Successful");
        this.productModel = {
          Name: '',
          Description: '',
          Price: 0,
          CategoryId: 0,
          IsPublic: true,
          SpecAttributes: [],
          SpecInventories: [],
          WarehouseId: 0,
          TotalInventory: 0,
          CoverImage: null,
          CoverImageUrl: null,
          Slug: null,
          CreatedAt: '',
          UpdatedAt: ''
        };
        this.category = {
          categoryId: null,
          name: "temp",
          isNext: false
        };

      },
      error: (err) => {
        this.messager.showModal("fail", err);
      }
    });
  }

  updateProduct(){
    let productId = 0;
    if (this.productId){
      productId = parseInt(this.productId);
    }
    if (productId === 0){
      throw new Error("Product is not found");
    }
    this.shopSVC.updateProductById(productId,this.productModel)
    .subscribe({
      next: (res) => {
        this.productModel.SpecAttributes.forEach(spec => {
          spec.SpecItems.forEach(specItem => {
            if (specItem.SpecImage){
              specItem.SpecImage = null;
            }
          });
        });
        this.messager.showModal("success", "Update Product Successful");
      },
      error: (err) => {
        this.messager.showModal("fail", "Update Product Fail");
        console.log(err);
      }
    });
  }

  setDetailImage(event: Event){
    const targetElement = event.currentTarget as HTMLImageElement;
    const selectedImageElement = ((targetElement.parentElement as HTMLElement).previousSibling as HTMLElement).firstChild as HTMLImageElement;
    const currentSrc = targetElement.src;
    selectedImageElement.src = currentSrc;
    selectedImageElement.classList.add("slide-in");
    setTimeout(() => {
      selectedImageElement.classList.remove("slide-in");
    }, 500);
  }

  deleteProduct(){
    let isDeleting = confirm("Are you sure to delete this product?");
    if (!isDeleting){
      return;
    }
    if (this.productId == null){
      throw new Error("Product isn't detail status");
    }
    let numId = parseInt(this.productId);
    this.shopSVC.deleteProduct(numId).subscribe({
      next: (res) => {
        this.messager.showModal("success", "Delete Product Successful");
        this.router.navigate(['shop-owner', this.shopId, 'products']);
      },
      error: (err) => {
        this.messager.showModal("fail", "Delete Product Fail");
        console.log(err);
      }
    });
  }
}

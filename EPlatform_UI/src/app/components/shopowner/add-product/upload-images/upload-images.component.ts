import { Component, EventEmitter, inject, Input, OnChanges, OnInit, Output, output, Renderer2, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductPostModel, SpecAttribute } from '../../models/product-post-model';
import { spec } from 'node:test/reporters';
import { ProductCreateUpdateModel } from '../../models/product-create-update-model';
import { DOCUMENT } from '@angular/common';

export interface FileUpload {
  file: File;
  SpecName: string;
}

@Component({
  selector: 'app-upload-images',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './upload-images.component.html',
  styleUrl: './upload-images.component.scss'
})

export class UploadImagesComponent {
  renderer = inject(Renderer2);
  
  files: FileUpload[] = [];
  currentFile: File | null = null;
  document = inject(DOCUMENT);
  
  @Output() upload = new EventEmitter<ProductCreateUpdateModel>();

  @Input() productModel: ProductCreateUpdateModel = {
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
    Slug: null
  }

  showBoard() {
    let boardElement = document.getElementById('upload-file-board');
    let curtain = document.getElementById('curtain');
    if (boardElement && curtain) {
      boardElement.classList.add('show');
      curtain.classList.add('show');
    }

    this.document.querySelectorAll(".image-preview").forEach((ele) => {
      const span = ele.previousSibling as HTMLElement;
      const img = ele as HTMLImageElement;
      if (img.naturalWidth !== 0) {
        this.renderer.addClass(span, 'hide');
        this.renderer.addClass(img, 'show');
      }
    });
  }

  saveCategory(){
    console.log(this.productModel);
    this.upload.emit(this.productModel);
    this.hideBoard();
  }

  showPreview(ele: HTMLElement) {
    let targetElement = ele.querySelector('label') as HTMLElement;
    const img = targetElement.querySelector('img') as HTMLImageElement;
    const span = targetElement.querySelector('span') as HTMLSpanElement;
    if (this.currentFile) {
      const imageUrl = URL.createObjectURL(this.currentFile);
      img.src = imageUrl;
      span.classList.add('hide');
      img.classList.add('show');
    } else {
      img.src = '';
      span.classList.remove('hide');
      img.classList.remove('show');
    }
  }

  hideBoard() {
    let boardElement = document.getElementById('upload-file-board');
    let curtain = document.getElementById('curtain');
    if (boardElement && curtain) {
      boardElement.classList.remove('show');
      curtain.classList.remove('show');
    }
  }

  saveFile(event: Event){
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.currentFile = input.files[0];
    }
  }

  onFileChange(event: Event, isCover: boolean = true, specValue: string = '') {
    const input = event.target as HTMLInputElement;
    this.saveFile(event);

    if (isCover){
      this.productModel.CoverImage = this.currentFile;
    } else {
      this.productModel.SpecAttributes.forEach(specAttribute => {
        if (specAttribute.IsPrimary){
          specAttribute.SpecItems.forEach(specItem => {
            if (specItem.SpecValue === specValue){
              specItem.SpecImage = this.currentFile;
            }
          });
        }
      });
    }

    if (input.parentElement) {
      this.showPreview(input.parentElement);
    }
  }
}

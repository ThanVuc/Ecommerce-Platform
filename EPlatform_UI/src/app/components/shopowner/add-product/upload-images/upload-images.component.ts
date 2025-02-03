import { Component, EventEmitter, Input, OnChanges, OnInit, Output, output, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductPostModel, SpecAttribute } from '../../models/product-post-model';
import { spec } from 'node:test/reporters';

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
  
  files: FileUpload[] = [];
  currentFile: File | null = null;
  
  @Output() upload = new EventEmitter<ProductPostModel>();

  @Input() productModel: ProductPostModel = {
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

  showBoard() {
    let boardElement = document.getElementById('upload-file-board');
    let curtain = document.getElementById('curtain');
    if (boardElement && curtain) {
      boardElement.classList.add('show');
      curtain.classList.add('show');
    }
  }

  saveCategory(){
    this.upload.emit(this.productModel);
    this.hideBoard();
  }

  showPreview(ele: HTMLElement, specName: string) {
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

  onFileChange(event: Event, specValue: string) {
    {
      const input = event.target as HTMLInputElement;
      if (input.files && input.files.length > 0) {
        this.currentFile = input.files[0];
        let item = this.productModel.specAttributes[0].specItems.find(item => item.specValue == specValue);
        if (item) {
          item.specImage = this.currentFile;
        }
      }

      if (input.parentElement) {
        this.showPreview(input.parentElement, specValue);
      }
    }
  }
}

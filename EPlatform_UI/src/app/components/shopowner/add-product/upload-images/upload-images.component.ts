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
      this.productModel.CoverImage = this.currentFile;
    }
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.saveFile(event);

    if (input.parentElement) {
      this.showPreview(input.parentElement);
    }
  }
}

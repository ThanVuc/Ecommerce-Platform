import { Component, OnInit } from '@angular/core';
import { SpecAttribute } from '../models/spec-attribute';
import { FormsModule } from '@angular/forms';
import { HtmlParser } from '@angular/compiler';
import { UpperCasePipe } from '@angular/common';
import e from 'express';
import { SelectCategoryComponent } from "./select-category/select-category.component";
import { CategoryModel } from '../models/category-model';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [FormsModule, SelectCategoryComponent],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.scss'
})

export class AddProductComponent implements OnInit {
  ngOnInit(): void {
    this.specList.push({
      specName: 'Color',
      specValues: ['Red', 'Blue', 'Green']
    },{
      specName: 'Size',
      specValues: ['X', 'L', 'XL']
    });
  }

  specList: SpecAttribute[] = [];
  category: CategoryModel = {
    categoryId: null,
    name: "temp",
    isNext: false
  }

  saveCategory(category: CategoryModel){
    this.category = category;
    console.log(this.category);
  }

  findCurrentSpec(event: Event){
    const targetElement = event.target as HTMLElement;
    const parentElement = targetElement.parentElement;
    const specNameElement = parentElement?.querySelector('.name') as HTMLInputElement;
    
    if (this.specList.find(spec => spec.specName === specNameElement.value) === undefined){
      return '';
    }
    
    return specNameElement.value;
  }

  triggerError(inputElement: HTMLInputElement){
    inputElement.classList.add("trigger-error");
    inputElement.addEventListener("animationend", () => {
      inputElement.classList.remove("trigger-error");
    }, {once: true});
  }

  saveSpec(event:Event){
    event.preventDefault();
    const valueInputElement = event.target as HTMLInputElement;
    const currentSpec = this.findCurrentSpec(event);
    console.log(currentSpec);
    let specValue = this.specList
    .find(spec => spec.specName === currentSpec)
    ?.specValues;
    
    if (specValue?.indexOf(valueInputElement.value) !== -1){
      this.triggerError(valueInputElement);
      return;
    }

    specValue?.push(valueInputElement.value);


    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    const valueElement = productSpecElement?.querySelector(".value") as HTMLInputElement;
    valueElement?.classList.toggle("hide");
    valueElement.value = '';
    productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");

  }

  addSpec(event: Event){
    if (this.specList.find(spec => spec.specName === '') !== undefined){
      const targetElement = event.target as HTMLInputElement;
      this.triggerError(targetElement);
      return;
    }

    this.specList.push({
      specName: '',
      specValues: []
    });
  }

  saveSpecName(event: Event,name: string){
    event.preventDefault();
    const targetElement = event.target as HTMLInputElement;
    let specObject = this.specList.find(spec => spec.specName === name);
    if (specObject) {
      console.log(this.specList);
      specObject.specName = targetElement.value;
    }
  }

  removeSpecValue(name: string, value: string){
    let specValues = this.specList.find(spec => spec.specName === name)?.specValues;
    specValues?.splice(specValues.indexOf(value), 1);
  }

  removeSpec(specName: string){
    const specIndex = this.specList.findIndex(spec => spec.specName === specName);
    if (specIndex !== -1) {
      this.specList.splice(specIndex, 1);
    }
  }

  showSpecValue(event: Event){
    event.preventDefault();
    const targetElement = event.target as HTMLElement;
    const productSpecElement = targetElement.parentElement;

    productSpecElement?.querySelector(".value")?.classList.toggle("hide");
    productSpecElement?.querySelector(".add-spec-value")?.classList.toggle("show");
  }
}

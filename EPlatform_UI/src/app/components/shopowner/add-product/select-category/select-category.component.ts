import { Component, EventEmitter, inject, OnInit, Output, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryModel } from '../../models/category-model';
import { ShopService } from '../../../services/shop.service';
import { UtilitiesServiceService } from '../../../services/utilities-service.service';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-select-category',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './select-category.component.html',
  styleUrl: './select-category.component.scss'
})
export class SelectCategoryComponent implements OnInit {
  ngOnInit(): void {
    this.loadCategories(this.currentCategory, this.rootOrder);
  }

  @Output() categorySelected = new EventEmitter<CategoryModel>();

  rootOrder: number = 1;
  searchString: string | null = null;
  categoryMap: Map<number | null, CategoryModel[]> = new Map<number, CategoryModel[]>();
  Utilities = inject(UtilitiesServiceService);
  document = inject(DOCUMENT);
  currentCategory: CategoryModel = {
    categoryId: null,
    name: "Select Category",
    isNext: false
  };

  showCategories() {
    let category_board = document.getElementById("category_board");
    let curtain = document.getElementById("curtain");
    if (category_board && curtain) {
      category_board.style.display = "flex";
      curtain.style.display = "block";
    }
  }

  loadSubCategories(event: Event, parentCategory: CategoryModel, order: number) {
    this.loadCategories(parentCategory, order);
    this.setCurrentCategory(event, parentCategory, order - 1);
  }

  setCurrentCategory(event: Event, category: CategoryModel, currentOrder: number) {
    const eventElement = event.target as HTMLElement;
    const btnElement = eventElement.parentElement as HTMLElement;
    this.currentCategory = category;
    const groupTarget = document.getElementById(`group-${currentOrder}`);
    if (btnElement) {
      groupTarget?.querySelectorAll(".category-item").forEach((element) => {
        element.classList.remove("active");
      });
      btnElement.classList.add("active");
    }
  }

  setCategory(category: CategoryModel) {
    this.currentCategory = category;
    this.saveCategory();
  }

  saveCategory() {
    this.passCategory();
    let mainBtnElement = this.document.querySelector(".button-category");
    if (mainBtnElement) {
      let p = mainBtnElement.querySelector("p");
      if (p && this.currentCategory) {
        p.textContent = this.currentCategory?.name;
      }
    }
  }

  passCategory() {
    if (this.currentCategory) {
      this.categorySelected.emit(this.currentCategory);
    }
    this.hideCategories();
  }

  hideCategories() {
    let category_board = this.document.getElementById("category_board");
    let curtain = this.document.getElementById("curtain");

    if (category_board && curtain) {
      category_board.style.display = "none";
      curtain.style.display = "none";
    }
  }

  loadCategories(parentCategory: CategoryModel, order: number) {
    if (this.categoryMap.has(order)) {
      console.log("order: " + order);
      let totalCategoryLayers = 4;
      for (let i = order; i <= totalCategoryLayers; i++) {
        this.categoryMap.delete(i);
      }
    }

    this.Utilities.getCategories(parentCategory.categoryId).subscribe({
      next: (res) => {
        this.categoryMap.set(order, res.data);
      },
      error: (err) => {
        console.log(err);
      }
    });
  }
}

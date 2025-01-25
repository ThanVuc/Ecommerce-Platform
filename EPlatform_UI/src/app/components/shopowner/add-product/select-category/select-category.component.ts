import { Component, EventEmitter, inject, OnInit, Output, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryModel } from '../../models/category-model';
import { ShopService } from '../../../services/shop.service';

@Component({
  selector: 'app-select-category',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './select-category.component.html',
  styleUrl: './select-category.component.scss'
})
export class SelectCategoryComponent implements OnInit {
  ngOnInit(): void {
    let categoryModel: CategoryModel = {
      categoryId: null,
      name: "temp",
      isNext: false
    };
    this.loadCategories(categoryModel, this.rootOrder);
  }

  @Output() categorySelected = new EventEmitter<CategoryModel>();

  rootOrder: number = 1;
  searchString: string = "";
  categoryMap: Map<number | null, CategoryModel[]> = new Map<number, CategoryModel[]>();
  shopSVC = inject(ShopService);
  currentCategory: CategoryModel | null = null;

  showCategories(){
    let category_board = document.getElementById("category_board");
    if (category_board){
      category_board.style.display = "flex";
    }
  }

  loadSubCategories(event:Event, parentCategory: CategoryModel, order: number){
    this.loadCategories(parentCategory, order);
    this.setCurrentCategory(event, parentCategory, order-1);
  }

  setCurrentCategory(event:Event, category: CategoryModel, currentOrder: number){
    const eventElement = event.target as HTMLElement;
    const btnElement = eventElement.parentElement as HTMLElement;
    this.currentCategory = category;
    const groupTarget = document.getElementById(`group-${currentOrder}`);
    if (btnElement){
      groupTarget?.querySelectorAll(".category-item").forEach((element) => {
        element.classList.remove("active");
      });
      btnElement.classList.add("active");
    }
  }

  hideCategories(){
    let category_board = document.getElementById("category_board");
    if (category_board){
      category_board.style.display = "none";
    }
  }

  saveCategory(){
    this.passCategory();
  }

  passCategory(){
    if (this.currentCategory){
      this.categorySelected.emit(this.currentCategory);
    }
    this.hideCategories();
  }

  loadCategories(parentCategory: CategoryModel | null, order: number){
    if (this.categoryMap.has(order)){
      console.log("order: "+order);
      let totalCategoryLayers = 4;
      for (let i = order; i <= totalCategoryLayers; i++){
        this.categoryMap.delete(i);
      }
    }
    if (parentCategory){
      this.shopSVC.getCategories(parentCategory.categoryId).subscribe({
        next: (res) => {
          this.categoryMap.set(order, res.data);
        },
        error: (err) => {
          console.log(err);
        }
      });
    }
  }
}

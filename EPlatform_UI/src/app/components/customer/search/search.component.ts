import { Component, inject, OnInit } from '@angular/core';
import { PaginationComponent } from "../../../shares/reusable/pagination/pagination.component";
import { ProductService } from '../../services/product.service';
import { searchProductModel } from '../models/search-product-model';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';
import { distinctUntilChanged, map } from 'rxjs';
import { PageModel } from '../../models/PageModel';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [PaginationComponent,RouterLink],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent implements OnInit {
  ngOnInit(): void {
    this.activatedRoute.queryParams
    .pipe(distinctUntilChanged())
    .subscribe({
      next: (params) => {
        const searchString = params['SearchString'] || "";
        const categoryId = params['CategoryId'] || 0;
        const categoryName = params['Name'] || "";
        if (
          searchString !== this.searchString ||
          categoryId !== this.categoryId ||
          categoryName !== this.categoryName
        ) {
          this.searchString = searchString;
          this.categoryId = categoryId;
          this.categoryName = categoryName;
          if (this.isInit){
            this.loadProducts({
              pageIndex: this.pageIndex,
              pageSize: this.pageSize
            });
          }
          this.isInit = true;
        }

      },
      error: (error) => {
        console.log(error);
      }
    });
  }
  productSVC = inject(ProductService);
  searchString: string = "";
  categoryId: number  = 0;
  categoryName: string = "";
  products: searchProductModel[] = [];
  pageIndex: number = 1;
  pageSize: number = 42;
  totalItem: number = 0;
  activatedRoute = inject(ActivatedRoute);
  isInit: boolean = false;

  loadProducts(pageModel: PageModel, isInit: boolean = false) {
    this.pageIndex = pageModel.pageIndex;
    this.pageSize = pageModel.pageSize;
    this.productSVC.searchProduct(this.pageIndex,this.pageSize,this.searchString, this.categoryId)
    .pipe(
      map(res => {
        const paginationInfo = res.headers.get("X-Pagination");
        let data: PaginationInfoModel | null = null;
        if (paginationInfo != null) {
          data = JSON.parse(paginationInfo);
        }

        if (data?.TotalItem) {
          this.totalItem = data.TotalItem;
        }

        return res.body;
      })
    )
    .subscribe({
      next: (res) => {
        if (res) {
          this.products = res.data;
        }
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
}

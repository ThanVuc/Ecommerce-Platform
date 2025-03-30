import { Component, inject, OnInit } from '@angular/core';
import { PaginationComponent } from "../../../shares/reusable/pagination/pagination.component";
import { ProductService } from '../../services/product.service';
import { searchProductModel } from '../models/search-product-model';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';
import { map } from 'rxjs';
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
    this.activatedRoute.queryParams.subscribe({
      next: (params) => {
        this.searchString = params['SearchString'] || "";
        this.categoryId = params['CategoryId'] || 0;
        this.categoryName = params['Name'] || "";
        this.loadProducts({
          pageIndex: this.pageIndex,
          pageSize: this.pageSize
        });
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

  loadProducts(pageModel: PageModel) {
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

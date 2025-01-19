import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { PaginationComponent } from '../../../shares/reusable/pagination/pagination.component';
import { FormsModule } from '@angular/forms';
import { PageModel } from '../../models/PageModel';
import { ShopService } from '../../services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { ProductModel } from '../models/product-model';
import { map } from 'rxjs';
import { PaginationInfoModel } from '../../models/PaginationInfoModel';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule, PaginationComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  ngOnInit(): void {
    this.activatedRoute.parent?.params.subscribe(params => { 
      this.shopId = params['shop_id'];
    });
  }

  @ViewChild(PaginationComponent) paginator!: PaginationComponent;

  searchString: string = "";
  activatedRoute = inject(ActivatedRoute);
  pageIndex: number = 1;
  limit: number = 5;
  totalItem: number = 10;
  shopOwnerSVC = inject(ShopService);
  shopId: string = "";
  products: ProductModel[] | undefined = [];
  timer: NodeJS.Timeout | null = null;

  loadPage(pageModel: PageModel) {
    this.pageIndex = pageModel.pageIndex;
    this.limit = pageModel.pageSize;
    this.loadProducts();
  }

  loadProducts() {
    this.shopOwnerSVC.getProductsByShopId(this.shopId,this.pageIndex,this.limit, this.searchString)
    .pipe(
      map(res => {
        var pagingInfoString = res.headers.get("X-Pagination");
        let pagingData: PaginationInfoModel | null = null;
        if (pagingInfoString != null) {
          pagingData = JSON.parse(pagingInfoString);
        }

        if (pagingData?.TotalItem) {
          this.totalItem = pagingData.TotalItem;
        }

        return res.body;
      })
    ).subscribe({
      next: (res) => {
        this.products = res?.data;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  switchStatus(productId: number, isPublic: boolean) {
    this.shopOwnerSVC.publicOrHideProduct(this.shopId, productId, !isPublic)
    .subscribe({
      next: (res) => {
        
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

}

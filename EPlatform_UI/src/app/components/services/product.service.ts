import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ApiResModel } from '../models/api-res-model';
import { RootCategoryModel } from '../customer/models/root-category-model';
import { ProductBriefModel } from '../customer/models/product-brief-model';
import { ProductDetailComponent } from '../customer/product-detail/product-detail.component';
import { ProductDetailModel } from '../customer/models/product-detail-model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  getCategoriesInHome() : Observable<ApiResModel<RootCategoryModel[]>> {
    return this.http.get<ApiResModel<RootCategoryModel[]>>(environment.getCategoriesInHome);
  }

  getHotProducts() : Observable<ApiResModel<ProductBriefModel[]>> {
    return this.http.get<ApiResModel<ProductBriefModel[]>>(environment.HotProduct);
  }

  getProductTodaySuggestions() : Observable<ApiResModel<ProductBriefModel[]>> {
    return this.http.get<ApiResModel<ProductBriefModel[]>>(environment.TodaySuggestion);
  }

  getProductDetail(productId: number) : Observable<ApiResModel<ProductDetailModel>> {
    return this.http.get<ApiResModel<ProductDetailModel>>(environment.getProductDetail + productId);
  }
}


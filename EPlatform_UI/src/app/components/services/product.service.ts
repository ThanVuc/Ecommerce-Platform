import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ApiResModel } from '../models/api-res-model';
import { RootCategoryModel } from '../customer/models/root-category-model';
import { ProductBriefModel } from '../customer/models/product-brief-model';
import { ProductDetailComponent } from '../customer/product-detail/product-detail.component';
import { ProductDetailModel } from '../customer/models/product-detail-model';
import { AddToCartModel } from '../customer/models/add-to-cart-model';
import { CartItemModel } from '../customer/models/cart-item-model';
import { searchProductModel } from '../customer/models/search-product-model';
import { SuggestionModel } from '../customer/models/suggestion-model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  getCategoriesInHome() : Observable<ApiResModel<RootCategoryModel[]>> {
    return this.http.get<ApiResModel<RootCategoryModel[]>>(environment.getCategoriesInHome);
  }

  getHotProducts() : Observable<ApiResModel<ProductBriefModel[]>> {
    return this.http.get<ApiResModel<ProductBriefModel[]>>(environment.HotProduct, {
      headers: { 'Cache-Control': 'no-cache', 'Pragma': 'no-cache' }
    });
  }

  getProductTodaySuggestions() : Observable<ApiResModel<ProductBriefModel[]>> {
    return this.http.get<ApiResModel<ProductBriefModel[]>>(environment.TodaySuggestion, {
      headers: { 'Cache-Control': 'no-cache', 'Pragma': 'no-cache' }
    });
  }

  getProductDetail(productId: number) : Observable<ApiResModel<ProductDetailModel>> {
    return this.http.get<ApiResModel<ProductDetailModel>>(environment.getProductDetail + productId);
  }

  addToCart(addToCartModel: AddToCartModel) : Observable<ApiResModel<object>> {
    return this.http.post<ApiResModel<object>>(environment.addToCart, addToCartModel);
  }

  getCartItems() : Observable<ApiResModel<CartItemModel[]>> {
    return this.http.get<ApiResModel<CartItemModel[]>>(environment.getCartItems);
  }

  removeCartItem(cartItemId: number) : Observable<ApiResModel<object>> {
    return this.http.delete<ApiResModel<object>>(environment.getCartItems+ `/${cartItemId}/remove-item`);
  }

  getCartNum() : Observable<ApiResModel<number>> {
    return this.http.get<ApiResModel<number>>(environment.getCartNum);
  }

  searchProduct(pageIndex: number, pageSize: number, searchString: string = "", categoryId: number) : Observable<HttpResponse<ApiResModel<searchProductModel[]>>> {
    let url = environment.searchProduct+`?PageNumber=${pageIndex}&PageSize=${pageSize}&SearchString=${searchString}&CategoryId=${categoryId}`;
    return this.http.get<ApiResModel<searchProductModel[]>>(url,{observe: 'response'});
  }

  getSuggestions(prefix: string) : Observable<ApiResModel<SuggestionModel[]>> {
    return this.http.get<ApiResModel<SuggestionModel[]>>(environment.getSuggestions + `?prefix=${prefix}`);
  }
}


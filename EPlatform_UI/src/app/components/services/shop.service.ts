import { HttpClient, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResModel } from '../models/api-res-model';
import { ProductModel } from '../shopowner/models/product-model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CategoryModel } from '../shopowner/models/category-model';

@Injectable({
  providedIn: 'root'
})
export class ShopService {

  constructor() { }

  http = inject(HttpClient);

  getProductsByShopId(shopId: string, pageIndex: number, limit: number, searchString: string | null = null): Observable<HttpResponse<ApiResModel<ProductModel[]>>> {
    let url = environment.Shop+shopId+`/products?PageNumber=${pageIndex}&PageSize=${limit}`;

    if (searchString != null) {
      url += `&searchString=${searchString}`;
    }
    
    return this.http.get<ApiResModel<ProductModel[]>>(url, { observe: 'response' });
  }

  publicOrHideProduct(shopId: string ,productId: number, isPublic: boolean): Observable<ApiResModel<object>> {
    return this.http.put<ApiResModel<object>>(environment.Shop+`${shopId}/products/public-or-hide-product`, {productId: productId, isPublic: isPublic});
  }

  getCategories(parentCategoryId: number | null): Observable<ApiResModel<CategoryModel[]>> {
    let queryString = parentCategoryId == null ? "" : `?parentCategoryId=${parentCategoryId}`;
    return this.http.get<ApiResModel<CategoryModel[]>>(environment.Categories + queryString);
  }
    
}

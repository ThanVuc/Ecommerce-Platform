import { HttpClient, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResModel } from '../models/api-res-model';
import { ProductModel } from '../shopowner/models/product-model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CategoryModel } from '../shopowner/models/category-model';
import { ProductPostModel } from '../shopowner/models/product-post-model';
import { ProductCreateUpdateModel } from '../shopowner/models/product-create-update-model';
import { ProductUpdateResponse } from '../shopowner/models/product-update-response';
import { env } from 'node:process';

@Injectable({
  providedIn: 'root'
})
export class ShopService {

  constructor() { }

  http = inject(HttpClient);

  getProductsByShopId(shopId: string, pageIndex: number, limit: number, searchString: string | null = null): Observable<HttpResponse<ApiResModel<ProductModel[]>>> {
    let url = environment.Shop + shopId + `/products?PageNumber=${pageIndex}&PageSize=${limit}`;

    if (searchString != null) {
      url += `&searchString=${searchString}`;
    }

    return this.http.get<ApiResModel<ProductModel[]>>(url, { observe: 'response' });
  }

  publicOrHideProduct(shopId: string, productId: number, isPublic: boolean): Observable<ApiResModel<object>> {
    return this.http.put<ApiResModel<object>>(environment.Shop + `${shopId}/products/public-or-hide-product`, { productId: productId, isPublic: isPublic });
  }

  addProduct(shopId: string, product: ProductCreateUpdateModel): Observable<ApiResModel<object>> {
    const formData = new FormData();

    formData.append('Name', product.Name);
    formData.append('CategoryId', product.CategoryId.toString());
    formData.append('Description', product.Description);
    formData.append('Price', product.Price.toString());
    formData.append('IsPublic', product.IsPublic.toString());
    formData.append('WarehouseId', product.WarehouseId.toString());
    formData.append('TotalInventory', product.TotalInventory.toString());
    if (product.CoverImage){
      formData.append('CoverImage', product.CoverImage);
    }

    product.SpecAttributes.forEach((specAttribute, index) => {
      formData.append(`SpecAttributes[${index}].SpecName`, specAttribute.SpecName);
      formData.append(`SpecAttributes[${index}].IsPrimary`, specAttribute.IsPrimary.toString());

      specAttribute.SpecItems.forEach((specItem, itemIndex) => {
        formData.append(`SpecAttributes[${index}].SpecItems[${itemIndex}].SpecValue`, specItem.SpecValue);

        if (specItem.SpecImage) {
          formData.append(`SpecAttributes[${index}].SpecItems[${itemIndex}].SpecImage`, specItem.SpecImage);
        }
      });
    });

    product.SpecInventories.forEach((specInventory, index) => {
      formData.append(`SpecInventories[${index}].PrimarySpecValueName`, specInventory.PrimarySpecValueName);
      formData.append(`SpecInventories[${index}].SubSpecValueName`, specInventory.SubSpecValueName);
      formData.append(`SpecInventories[${index}].Inventory`, specInventory.Inventory.toString());
    });

    return this.http.post<ApiResModel<object>>(environment.AddProduct + `${shopId}/products/add-product`, formData);
  }

  getUpdateProduct(productId: string): Observable<ApiResModel<ProductUpdateResponse>> {
    return this.http.get<ApiResModel<ProductUpdateResponse>>(environment.UpdateProduct + `${productId}/update`);
  }

  updateProductById(productId: number, product: ProductCreateUpdateModel): Observable<ApiResModel<object>> {
    const formData = new FormData();

    formData.append('Name', product.Name);
    formData.append('CategoryId', product.CategoryId.toString());
    formData.append('Description', product.Description);
    formData.append('Price', product.Price.toString());
    formData.append('IsPublic', product.IsPublic.toString());
    formData.append('WarehouseId', product.WarehouseId.toString());
    formData.append('TotalInventory', product.TotalInventory.toString());
    if (product.CoverImage){
      formData.append('CoverImage', product.CoverImage);
    }

    product.SpecAttributes.forEach((specAttribute, index) => {
      formData.append(`SpecAttributes[${index}].SpecName`, specAttribute.SpecName);
      formData.append(`SpecAttributes[${index}].IsPrimary`, specAttribute.IsPrimary.toString());

      specAttribute.SpecItems.forEach((specItem, itemIndex) => {
        formData.append(`SpecAttributes[${index}].SpecItems[${itemIndex}].SpecValue`, specItem.SpecValue);

        if (specItem.SpecImage) {
          formData.append(`SpecAttributes[${index}].SpecItems[${itemIndex}].SpecImage`, specItem.SpecImage);
        }
      });
    });

    product.SpecInventories.forEach((specInventory, index) => {
      formData.append(`SpecInventories[${index}].PrimarySpecValueName`, specInventory.PrimarySpecValueName);
      formData.append(`SpecInventories[${index}].SubSpecValueName`, specInventory.SubSpecValueName);
      formData.append(`SpecInventories[${index}].Inventory`, specInventory.Inventory.toString());
    });

    return this.http.put<ApiResModel<object>>(environment.UpdateProduct + `${productId}/update`, formData);
  }

}

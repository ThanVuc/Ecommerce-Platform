import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResModel } from '../models/api-res-model';
import { CategoryModel } from '../shopowner/models/category-model';
import { firstValueFrom, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WarehouseItem } from '../shopowner/models/product-post-model';
import { LocationModel } from '../models/location-model';

@Injectable({
  providedIn: 'root'
})
export class UtilitiesServiceService {

  constructor() { }
  http = inject(HttpClient);

  async getWarehouses(){
    return firstValueFrom(this.http.get<ApiResModel<WarehouseItem[]>>(environment.WarehouseForSelect));
  }

  getCategories(parentCategoryId: number | null): Observable<ApiResModel<CategoryModel[]>> {
    let queryString = parentCategoryId == null ? "" : `?parentCategoryId=${parentCategoryId}`;
    return this.http.get<ApiResModel<CategoryModel[]>>(environment.Categories + queryString);
  }

  getProvices(): Observable<ApiResModel<LocationModel[]>> {
    return this.http.get<ApiResModel<LocationModel[]>>(environment.Province);
  }

  getDistricts(provinceCode: string): Observable<ApiResModel<LocationModel[]>> {
    return this.http.get<ApiResModel<LocationModel[]>>(environment.District + provinceCode);
  }

  getWards(districtCode: string): Observable<ApiResModel<LocationModel[]>> {
    return this.http.get<ApiResModel<LocationModel[]>>(environment.Ward + districtCode);
  }

}

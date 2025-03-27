import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrderModel } from '../customer/models/create-order-model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { OrderModel } from '../shopowner/orders/models/order-model';
import { ApiResModel } from '../models/api-res-model';
import { PageModel } from '../models/PageModel';
import { StatusModel } from '../shopowner/orders/models/status-model';
import { ChangeStatusModel } from '../shopowner/orders/models/change-status-model';
import { OrderDetailModel } from '../shopowner/orders/models/order-detail-model';
import { GetPurchaseOrdersModel } from '../customer/models/get-purchase-orders';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient) { }

  createOrder(createOrderModel: CreateOrderModel) {
    return this.http.post(environment.createOrder, createOrderModel);
  }

  getOrders(shopId: string, orderStatusId: number | null = null, pageModel: PageModel, searchString: string | null = null) : Observable<HttpResponse<ApiResModel<OrderModel[]>>> {
    let url = environment.Shop + `${shopId}/orders?`;
    if (orderStatusId){
      url += `OrderStatusId=${orderStatusId}&`;
    }
    if (searchString){
      url += `SearchString=${searchString}&`;
    }
    url += `PageNumber=${pageModel.pageIndex}&PageSize=${pageModel.pageSize}`;
    
    return this.http.get<ApiResModel<OrderModel[]>>(url,{observe: 'response'});
  }

  getAllStatuses() : Observable<ApiResModel<StatusModel[]>> {
    return this.http.get<ApiResModel<StatusModel[]>>(environment.getAllStatuses);
  }

  changeOrderStatus(OrderStatuses: ChangeStatusModel[]) : Observable<ApiResModel<object>> {
    return this.http.put<ApiResModel<object>>(environment.changeOrderStauts, OrderStatuses);
  }

  getOrderById(orderId: number) : Observable<ApiResModel<OrderDetailModel>> {
    return this.http.get<ApiResModel<OrderDetailModel>>(environment.getOrderById + orderId);
  }

  getPurchaseOrders(userId: string | null) : Observable<ApiResModel<GetPurchaseOrdersModel[]>>{
    let url = environment.getPurchaseOrders + `?userId=${userId}`;
    return this.http.get<ApiResModel<GetPurchaseOrdersModel[]>>(url);
  }

  cancelOrder(orderId: number) : Observable<ApiResModel<object>> {
    return this.http.put<ApiResModel<object>>(environment.cancelOrder + orderId, null);
  }
}

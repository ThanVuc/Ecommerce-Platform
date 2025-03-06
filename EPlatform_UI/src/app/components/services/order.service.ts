import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateOrderModel } from '../customer/models/create-order-model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient) { }

  createOrder(createOrderModel: CreateOrderModel) {
    return this.http.post(environment.createOrder, createOrderModel);
  }
}

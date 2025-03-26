import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderDetailModel } from '../models/order-detail-model';
import { OrderService } from '../../../services/order.service';
import { DatePipe, NgClass } from '@angular/common';
import { DataService } from '../../../services/data.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [RouterLink,DatePipe],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.scss'
})
export class OrderDetailComponent implements OnInit {
  constructor() {}
  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
      this.orderId = params['order_id'];
      this.returnStatusId = params['status_id'] ? parseInt(params['status_id']) : null;
      if (this.orderId) {
        this.getOrder();
      }
    });
  }

  orderSVC = inject(OrderService);
  orderId: number = 0;
  activatedRoute = inject(ActivatedRoute);
  dataSVC = inject(DataService);

  // sample order data
  order: OrderDetailModel = {
    orderId: 123,
    orderStatus: 'Preparing',
    createAt: 'January 1, 2025',
    accountName: 'John Doe',
    orderNums: 5,
    email: 'john.doe@example.com',
    phone: '+123456789',
    customerName: 'John Doe',
    shippingAddress: '123 Main St, City, Country',
    shippingPhone: '+123456789',
    products: [
      {
        avtImg: 'path/to/image.jpg',
        name: 'Product Name',
        quantity: 2,
        price: 50
      }
    ]
  };
  returnStatusId: number | null = null;

  getOrder(){
    this.orderSVC.getOrderById(this.orderId).subscribe({
      next: (res) => {
        this.order = res.data;
        console.log(this.order);
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  getTotalPrice(){
    return this.order.products.reduce((acc, cur) => acc + cur.price * cur.quantity, 0);
  }

  navigateBackOldStatus(){
    this.dataSVC.changeStatusId(this.returnStatusId);
  }
}

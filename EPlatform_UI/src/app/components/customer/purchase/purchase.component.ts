import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { ActivatedRoute } from '@angular/router';
import { GetPurchaseOrdersModel, Product } from '../models/get-purchase-orders';
import { DatePipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-purchase',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './purchase.component.html',
  styleUrl: './purchase.component.scss'
})
export class PurchaseComponent implements OnInit {
  constructor() { }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(params => {
      this.userId = params['userId'];
      this.getPurchaseOrders();
    });
  }

  orderSVC = inject(OrderService);
  userId: string | null = null;
  activatedRoute = inject(ActivatedRoute);
  orders: GetPurchaseOrdersModel[] = [];

  getPurchaseOrders() {
    this.orderSVC.getPurchaseOrders(this.userId).subscribe({
      next: (res) => {
        this.orders = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  getPurchaseOrderTotalCost(products: Product[]) {
    return products.reduce((acc, curr) => acc + curr.price, 0);
  }

  cancelOrder(orderId: number) {
    confirm('Are you sure you want to cancel this order?');
    this.orderSVC.cancelOrder(orderId).subscribe({
      next: (res) => {
        this.getPurchaseOrders();
      }
    });
  }

}

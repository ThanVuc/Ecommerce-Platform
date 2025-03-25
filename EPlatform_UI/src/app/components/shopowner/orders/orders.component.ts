import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { DataService } from '../../services/data.service';
import { StatusModel } from './models/status-model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  ngOnInit(): void {
    this.getAllStatuses();
  }
  dataSVC = inject(DataService);
  orderSVC = inject(OrderService);
  statuses: StatusModel[] = [];

  getAllStatuses() {
    this.orderSVC.getAllStatuses().subscribe(res => {
      this.statuses = res.data;
      this.dataSVC.changeMessage(res.data);
    });
  }

  getStatusByName(name: string){
    return this.statuses.find(x => x.statusName === name);
  }

  navigateAnotherStatus(event: Event ,statusId: number | null){
    const currentElement = event.currentTarget as HTMLElement;
    const parentElement = currentElement.parentElement;
    if (parentElement || currentElement) {
      parentElement?.querySelectorAll('.label').forEach((element) => {
        element.classList.remove('header-active');
      });
      currentElement.classList.add('header-active');
      this.dataSVC.changeStatusId(statusId);
    }
  }
}

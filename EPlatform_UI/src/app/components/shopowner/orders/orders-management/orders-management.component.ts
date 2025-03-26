import { Component, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PaginationComponent } from "../../../../shares/reusable/pagination/pagination.component";
import { PageModel } from '../../../models/PageModel';
import { DatePipe, DOCUMENT, NgClass } from '@angular/common';
import { ChangeStatusModel} from '../models/change-status-model';
import { DataService } from '../../../services/data.service';
import { OrderModel } from '../models/order-model';
import { OrderService } from '../../../services/order.service';
import { PaginationInfoModel } from '../../../models/PaginationInfoModel';
import { map } from 'rxjs';
import { send } from 'process';
import { StatusModel } from '../models/status-model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-orders-management',
  standalone: true,
  imports: [PaginationComponent, DatePipe, NgClass, FormsModule, RouterLink],
  templateUrl: './orders-management.component.html',
  styleUrl: './orders-management.component.scss'
})
export class OrdersManagementComponent implements OnInit {
  @ViewChild(PaginationComponent) paginator!: PaginationComponent;
  constructor() { }
  document = inject(DOCUMENT);
  ngOnInit() {
    this.dataSVC.currentMessage.subscribe(statuses => this.statuses = statuses);
    this.dataSVC.currentStatusId.subscribe({
      next: (statusId) => {
        this.statusId = statusId;
        this.loadPage({pageIndex: this.pageIndex, pageSize: this.pageSize} as PageModel);
      }
    });

    this.activatedRoute.parent?.parent?.params.subscribe(params => {
      this.shopId = params['shop_id'];
      this.activatedRoute.queryParams.subscribe(params => {
        this.statusId = params["status"] ? parseInt(params["status"]) : null;
      });
      this.loadPage({pageIndex: this.pageIndex, pageSize: this.pageSize} as PageModel);
    });
  }

  activatedRoute = inject(ActivatedRoute);
  dataSVC = inject(DataService);
  orderSVC = inject(OrderService);
  statuses: StatusModel[] = [];
  changedStatusList: ChangeStatusModel[] = [];
  orders: OrderModel[] = [];
  shopId: string = "";
  statusId: number | null = null;
  searchString: string | null = null;
  totalItem: number = 1;
  pageIndex: number = 1;
  pageSize: number = 10;
  
  loadPage(page: PageModel) {
    this.pageIndex = page.pageIndex;
    this.pageSize = page.pageSize;
    this.orderSVC.getOrders(this.shopId, this.statusId, page, this.searchString)
    .pipe(
      map(res => {
              const paginationInfo = res.headers.get("X-Pagination");
              let data: PaginationInfoModel | null = null;
              if (paginationInfo != null) {
                data = JSON.parse(paginationInfo);
              }
      
              if (data?.TotalItem) {
                this.totalItem = data.TotalItem;
              }
      
              return res.body;
            })
    ).subscribe({
      next: (res) => {
        if (res?.data) {
          this.orders = res.data;
        }
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  search(){
    console.log("enter search");
    this.loadPage({pageIndex: 1, pageSize: this.pageSize} as PageModel);
  }

  changeStatus(event: Event, originalStatus: string, orderId: number){
    const statusBtn = (event.target as HTMLElement);
    const selectStatus = (event.target as HTMLElement).nextElementSibling;
    const selectStatusClassName = ".select-status";
    console.log(selectStatus?.classList);
    if (statusBtn?.classList.contains('change')){
      statusBtn?.classList.remove('change');
      statusBtn.innerHTML = originalStatus;
      this.changedStatusList = this.changedStatusList.filter(x => x.OrderId !== orderId);
      return;
    }
    if (selectStatusClassName) {
      this.document.querySelectorAll(selectStatusClassName).forEach((element) => {
          element.classList.remove('show-select-status');
      });
    }
    selectStatus?.classList.add('show-select-status');
  }

  hideSelectStatus(event: Event){
    const selectStatus = (event.target as HTMLElement).closest('.select-status');
    selectStatus?.classList.remove('show-select-status');
  }

  setNewStatus(event: Event, orderId: number, statusId: number, status: string, ){
    this.changedStatusList = this.changedStatusList.filter(x => x.OrderId !== orderId);
    
    this.changedStatusList.push({
      StatusId: statusId,
      Status: status,
      OrderId: orderId
    });
    console.log(this.changedStatusList);
    const statusBtn = (event.target as HTMLElement).parentElement?.parentElement?.previousElementSibling;
    if (statusBtn) {
        statusBtn.textContent = status;
        statusBtn.classList.add('change');
    }
  }

  getNewStatus(orderId: number){
    const status = this.changedStatusList.find(x => x.OrderId === orderId);
    return status ? status.Status : "";
  }

  isStatusChanged(orderId: number){
    return this.changedStatusList.some(x => x.OrderId === orderId);
  }

  getStatusClass(orderId: number){
    return this.orders.find(x => x.orderId === orderId)?.orderStatusName.toLocaleLowerCase() || "preparing";
  }

  saveStatus(){
    this.orderSVC.changeOrderStatus(this.changedStatusList).subscribe({
      next: (res) => {
        if (res.data) {
          this.changedStatusList = [];
          this.loadPage({pageIndex: this.pageIndex, pageSize: this.pageSize} as PageModel);
          alert(res.data);
        } else {
          this.changedStatusList = [];
          this.loadPage({pageIndex: this.pageIndex, pageSize: this.pageSize} as PageModel);
        }
      }
    });
  }
}

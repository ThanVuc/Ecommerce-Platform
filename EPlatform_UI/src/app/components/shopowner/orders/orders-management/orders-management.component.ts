import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaginationComponent } from "../../../../shares/reusable/pagination/pagination.component";
import { PageModel } from '../../../models/PageModel';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-orders-management',
  standalone: true,
  imports: [PaginationComponent],
  templateUrl: './orders-management.component.html',
  styleUrl: './orders-management.component.scss'
})
export class OrdersManagementComponent implements OnInit {
  constructor() { }
  document = inject(DOCUMENT);
  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(params => {
      console.log(params["status"]);
    });
  }

  activatedRoute = inject(ActivatedRoute);
  statusCode = [
    {
      id: "01",
      name: "Preparing"
    },
    {
      id: "02",
      name: "Delivering"
    },
    {
      id: "03",
      name: "Completed"
    },
    {
      id: "04",
      name: "Canceled"
    }
  ];
  
  loadPage(page: PageModel) {
    console.log(page);
  }

  changeStatus(event: Event){
    const statusBtn = (event.target as HTMLElement);
    const selectStatus = (event.target as HTMLElement).nextElementSibling;
    const selectStatusClassName = ".select-status";
    console.log(selectStatus?.classList);
    if (statusBtn?.classList.contains('change')){
      statusBtn?.classList.remove('change');
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

  setNewStatus(event: Event, status: string){
    const statusBtn = (event.target as HTMLElement).parentElement?.parentElement?.previousElementSibling;
    if (statusBtn) {
        statusBtn.innerHTML = status;
        statusBtn.className = `status-btn ${status.toLowerCase()} change`;
    }
  }
}

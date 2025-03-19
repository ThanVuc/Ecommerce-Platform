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
  
  loadPage(page: PageModel) {
    console.log(page);
  }

  changeStatus(event: Event){
    const selectStatus = (event.target as HTMLElement).nextElementSibling;
    const selectStatusClassName = ".select-status";
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
}

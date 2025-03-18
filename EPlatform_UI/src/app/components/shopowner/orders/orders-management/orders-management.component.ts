import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-orders-management',
  standalone: true,
  imports: [],
  templateUrl: './orders-management.component.html',
  styleUrl: './orders-management.component.scss'
})
export class OrdersManagementComponent implements OnInit {
  constructor() { }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(params => {
      console.log(params["status"]);
    });
  }

  activatedRoute = inject(ActivatedRoute);

}

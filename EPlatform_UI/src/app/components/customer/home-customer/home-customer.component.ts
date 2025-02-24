import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home-customer',
  standalone: true,
  imports: [],
  templateUrl: './home-customer.component.html',
  styleUrl: './home-customer.component.scss'
})
export class HomeCustomerComponent implements OnInit {
  ngOnInit(): void {
    
  }
}

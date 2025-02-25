import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { TokenService } from '../../services/token.service';

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

  tokenSVC = inject(TokenService);

  start = 0;
  showCount = 6;

  categories = [
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 1",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 2",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 3",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 4",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 5",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 6",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 7",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 8",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 9",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 10",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 11",
    },
    {
      Img: "/assets/img/category.jpg",
      Name: "Category 12",
    },
  ]

  categoryChange(isPrev: boolean) {
    if (isPrev) {
      this.start = this.start - this.showCount;
    } else {
      this.start = this.start + this.showCount > this.categories.length ? this.start : this.start + this.showCount;
    }
  }


}

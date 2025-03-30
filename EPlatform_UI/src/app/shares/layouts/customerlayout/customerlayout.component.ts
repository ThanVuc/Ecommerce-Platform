import { DOCUMENT } from '@angular/common';
import { Component, inject, OnChanges, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TokenService } from '../../../components/services/token.service';
import { ShopService } from '../../../components/services/shop.service';
import { AuthService } from '../../../components/services/auth.service';
import { ProductService } from '../../../components/services/product.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-customerlayout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, FormsModule],
  templateUrl: './customerlayout.component.html',
  styleUrl: './customerlayout.component.scss'
})
export class CustomerlayoutComponent implements OnInit {
  ngOnInit(): void {
    this.renderer2.setStyle(this.document.body, 'background-color', 'black');
    this.role = this.tokenSVC.getRole();
    this.router.events.subscribe({
      next: (event) => {
        if (event instanceof NavigationEnd){
          this.url = event.url;
          if (this.tokenSVC.isAuthenicated()){
            this.productSVC.getCartNum().subscribe({
              next: (data) => {
                this.cartNum = data.data;
              },
              error: (error) => {
                console.log(error);
              }
            });
          }
        }
      },
      error: (error) => {
        console.log(error);
      }
    });

    if (this.tokenSVC.isAuthenicated()){
      this.productSVC.getCartNum().subscribe({
        next: (data) => {
          this.cartNum = data.data;
        },
        error: (error) => {
          console.log(error);
        }
      });

      this.shopSVC.getUserId().subscribe({
        next: (data) => {
          this.userId = data.data;
        },
        error: (error) => {
          console.log(error);
        }
      });
    }
  }
  productSVC = inject(ProductService);
  renderer2 = inject(Renderer2);
  document = inject(DOCUMENT);
  tokenSVC = inject(TokenService);
  router = inject(Router);
  shopSVC = inject(ShopService);
  authSVC = inject(AuthService);
  role: string[] | null = null;
  cartNum: number | null = null;
  url: string | null = null;
  userId: string | null = null;
  searchString: string | null = null;

  searchProduct() {
    this.router.navigateByUrl(`/search?SearchString=${this.searchString}`);
  }

  redirectToShop() {
    if (!this.tokenSVC.isAuthenicated()){
      this.router.navigateByUrl(`/shop-owner/unauthorized`);
      return;
    }

    if (this.role && this.role.includes('ShopOwner')) {
      this.router.navigateByUrl(`shop-owner/${this.userId}`);
    } else {
      this.router.navigateByUrl('shop-owner/create-shop');
    }
  }
  
  signOut(){
    this.authSVC.signOut();
    this.role = null;
  }
}

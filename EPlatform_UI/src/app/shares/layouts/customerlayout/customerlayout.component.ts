import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { TokenService } from '../../../components/services/token.service';
import { ShopService } from '../../../components/services/shop.service';
import { AuthService } from '../../../components/services/auth.service';

@Component({
  selector: 'app-customerlayout',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './customerlayout.component.html',
  styleUrl: './customerlayout.component.scss'
})
export class CustomerlayoutComponent implements OnInit {
  ngOnInit(): void {
    this.renderer2.setStyle(this.document.body, 'background-color', 'black');
    this.role = this.tokenSVC.getRole();
  }
  renderer2 = inject(Renderer2);
  document = inject(DOCUMENT);
  tokenSVC = inject(TokenService);
  router = inject(Router);
  shopSVC = inject(ShopService);
  authSVC = inject(AuthService);
  role: string[] | null = null;

  redirectToShop() {
    if (!this.tokenSVC.isAuthenicated()){
      this.router.navigateByUrl(`/shop-owner/unauthorized`);
      return;
    }

    if (this.role && this.role.includes('ShopOwner')) {
      const shopId = this.shopSVC.getUserId().subscribe({
        next: (data) => {
          this.router.navigateByUrl(`/shop-owner/${data.data}`);
        },
        error: (error) => {
          console.log(error);
        }
      });
    } else {
      this.router.navigateByUrl('shop-owner/create-shop');
    }
  }
  
  signOut(){
    this.authSVC.signOut();
  }
}

import { DOCUMENT } from '@angular/common';
import { Component, inject, OnChanges, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TokenService } from '../../../components/services/token.service';
import { ShopService } from '../../../components/services/shop.service';
import { AuthService } from '../../../components/services/auth.service';
import { ProductService } from '../../../components/services/product.service';
import { FormsModule } from '@angular/forms';
import { SuggestionModel } from '../../../components/customer/models/suggestion-model';

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
  searchString: string = "";
  timer!: NodeJS.Timeout | null;
  suggestions: SuggestionModel[] = [];

  searchProduct(suggestion: string | null = null) {
    if (suggestion){
      this.searchString = suggestion;
    }
    this.router.navigateByUrl(`/search?SearchString=${this.searchString}`);
  }

  debounce(event: Event) {
    event.preventDefault();
    event.stopPropagation();

    this.timer = null;
    this.timer = setTimeout(() => {
      // call api get suggestions
      this.productSVC.getSuggestions(this.searchString).subscribe({
        next: (res) => {
          this.suggestions = res.data;
          this.showSuggestions(event);
        },
        error: (error) => {
          console.log(error);
        }
      });
          
    }, 300);
  }

  showSuggestions(event: Event){
    event.preventDefault();
    event.stopPropagation();
    const suggestions = (event.target as HTMLInputElement).parentElement?.nextElementSibling as HTMLElement;
    if (suggestions){
      suggestions.classList.add('show');

      this.document.addEventListener('click', (e) => {
        if (e.target !== suggestions && e.target !== event.target){
          suggestions.classList.remove('show');
        }
      });
    }
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

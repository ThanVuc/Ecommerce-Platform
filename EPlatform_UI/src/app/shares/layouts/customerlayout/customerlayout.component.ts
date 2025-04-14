import { DOCUMENT } from '@angular/common';
import { Component, inject, OnChanges, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { ShopService } from '../../../components/services/shop.service';
import { AuthService } from '../../../components/services/auth.service';
import { ProductService } from '../../../components/services/product.service';
import { FormsModule } from '@angular/forms';
import { SuggestionModel } from '../../../components/customer/models/suggestion-model';
import { query } from 'express';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-customerlayout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, FormsModule],
  templateUrl: './customerlayout.component.html',
  styleUrl: './customerlayout.component.scss'
})
export class CustomerlayoutComponent implements OnInit {
  isAuthenticatedState: boolean = false; // New property to track authentication state

  async ngOnInit(): Promise<void> {
    this.renderer2.setStyle(this.document.body, 'background-color', 'black');
    await this.checkAuthenticated();
    this.router.events.subscribe({
      next: (event) => {
        if (event instanceof NavigationEnd){
          this.url = event.url;

          if (this.isAuthenticatedState){
            this.getCartNum();
          }
        }
      },
      error: (error) => {
        console.log(error);
      }
    });

    if (this.isAuthenticatedState){
      console.log("User is authenticated.");
      this.getCartNum();
      this.getUserId();
      this.isAdminCheck();
    }
  }
  productSVC = inject(ProductService);
  renderer2 = inject(Renderer2);
  document = inject(DOCUMENT);
  router = inject(Router);
  shopSVC = inject(ShopService);
  authSVC = inject(AuthService);
  cartNum: number | null = null;
  url: string | null = null;
  userId: string | null = null;
  searchString: string = "";
  timer!: NodeJS.Timeout | null;
  suggestions: SuggestionModel[] = [];
  isAdmin: boolean = false;

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
    // identify the role
    // if role include shop owner, redirect to shop page
    // if role != customer, redirect to customer page
    if (!this.isAuthenticatedState){
      this.router.navigateByUrl("/auth/login");
      return;
    }

    // check role
    this.authSVC.checkRole("ShopOwner").subscribe({
      next: (res) => {
        if (res.data){
          // add shop id to route
          if (this.userId) {
            this.router.navigate(["/shop-owner", this.userId]);
          } else {
            console.error("User ID is null. Cannot navigate to shop-owner.");
          }
        } else {
          this.router.navigateByUrl("/shop-owner/create-shop");
        }
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  getUserId(){
    this.shopSVC.getUserId().subscribe({
      next: (data) => {
        this.userId = data.data;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  getCartNum(){
    this.productSVC.getCartNum().subscribe({
      next: (data) => {
        this.cartNum = data.data;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
  
  signOut(){
    this.authSVC.signOut();
    this.isAuthenticatedState = false; // Reset state on sign-out
    this.isAdmin = false; // Reset admin state on sign-out
    this.cartNum = null; // Reset cart number on sign-out
  }

  async checkAuthenticated(): Promise<void> {
    try {
      await firstValueFrom(this.authSVC.IsAuthenticatedOrRefresh());
      this.isAuthenticatedState = true; // Update state on success
    } catch (err: any) {
      this.isAuthenticatedState = false; // Update state on failure
    }
  }

  isAdminCheck() {
    this.authSVC.checkRole("Admin").subscribe({
      next: (res) => {
        this.isAdmin = res.data;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

}

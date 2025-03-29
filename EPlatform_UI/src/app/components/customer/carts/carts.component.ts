import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { BuyProductComponent } from './buy-product/buy-product.component';
import { CartItemModel } from '../models/cart-item-model';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { CreateOrderModel } from '../models/create-order-model';
import { create } from 'domain';
import { MessageComponent } from "../../../shares/reusable/message/message.component";
import { SignalRService } from '../../services/signal-r.service';

@Component({
  selector: 'app-carts',
  standalone: true,
  imports: [BuyProductComponent, FormsModule, NgClass, MessageComponent],
  templateUrl: './carts.component.html',
  styleUrl: './carts.component.scss'
})
export class CartsComponent implements OnInit {
  @ViewChild(BuyProductComponent) buyProductComponent!: BuyProductComponent;
  @ViewChild(MessageComponent) messager!: MessageComponent;
  ngOnInit(): void {
    this.getCartItems();
  }

  productSVC = inject(ProductService);
  activatedRoute = inject(ActivatedRoute);
  orderSVC = inject(OrderService);
  router = inject(Router);
  signalRService = inject(SignalRService);

  cartItemsModel: CartItemModel[] = [];
  buyCartItemList: CartItemModel[] = [];
  isCarts = true;
  createOrderModel: CreateOrderModel = {
    email: '',
    phone: '',
    shippingAddress: '',
    cartItems: []
  }

  getCartItems(){
    this.productSVC.getCartItems().subscribe({
      next: (res) => {
        this.cartItemsModel = res.data;
        const buyNowCartItem = this.activatedRoute.snapshot.queryParamMap.get("cart_item_id");
        this.cartItemsModel.find((cartItem) => {
          if (cartItem.cartItemId === parseInt(buyNowCartItem as string)){
            cartItem.isSelected = true;
          }
        });
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  tickTheBox(event: Event, cartItem: CartItemModel){
      var checkSpanElement = (event.currentTarget as HTMLElement).firstChild as HTMLElement;
      cartItem.isSelected = !cartItem.isSelected;
      if (cartItem.isSelected){
        checkSpanElement.classList.add('checked');
      } else {
        checkSpanElement.classList.remove('checked');
      }
  }

  minusQuantity(cartItem: CartItemModel){
    if (cartItem.quantity <= 1){
      return
    }
    cartItem.quantity -= 1;
  }

  plusQuantity(cartItem: CartItemModel){
    if (cartItem.quantity >= cartItem.availableQuantity){
      return
    }
    cartItem.quantity += 1;
  }

  totalCost(){
    return this.cartItemsModel.reduce((acc, item) => item.quantity > 0 && item.isSelected ? acc + item.productPrice * item.quantity : acc, 0);
  }

  removeCartItem(event: Event, cartItemId: number){
    this.productSVC.removeCartItem(cartItemId).subscribe({
      next: (res) => {
        const cartItem = (event.target as HTMLElement).parentElement?.parentElement?.parentElement;
        if (cartItem){
          console.log(cartItem);
          cartItem.classList.add('fadeOut');
          setTimeout(() => {
            this.getCartItems();
          }, 500);
        }
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  buyProduct(){
    console.log(this.cartItemsModel);
    if (this.cartItemsModel.filter((cartItem) => cartItem.isSelected).length === 0){
      alert('Please select at least one product to buy');
      return;
    }
    this.isCarts = false;

    const appBuyProduct = document.getElementById('app-buy-product');
    const cartLists = document.getElementById('carts-list');
    if (appBuyProduct && cartLists){
      appBuyProduct.classList.remove('hide');
      cartLists.classList.add('hide');
    }
  }

  getPersonalInfo(createOrderModel: CreateOrderModel){
    this.createOrderModel = createOrderModel;
  }

  createOrder(){
    this.buyProductComponent.getPersonalInfoEvent();
    this.createOrderModel.cartItems = this.cartItemsModel.filter((cartItem) => cartItem.isSelected).map((cartItem) => {
      return {
        quantity: cartItem.quantity,
        productId: cartItem.productId,
        specInfo: (cartItem.specInfo as string) || '',
        shopId: cartItem.shopId,
        price: cartItem.productPrice,
        cartItemId: cartItem.cartItemId
      }
    });

    if (document.querySelectorAll('.err').length > 0){
      document.querySelectorAll('.err').forEach((err) => {
        err.classList.remove('hide');
      });
      return;
    }

    this.orderSVC.createOrder(this.createOrderModel).subscribe({
      next: (res) => {
        this.signalRService.sendNotification(this.cartItemsModel.filter((cartItem) => cartItem.isSelected).map((cartItem) => cartItem.shopId.toString()), `You have a new order from ${this.createOrderModel.email}`);
        alert('Order created successfully!');
        this.getCartItems();
        this.backToCart();
      },
      error: (err) => {
        alert('Error occurred while creating order');
        console.log(err);
      }
    });
  }

  backToCart(){
    this.isCarts = true;
    const appBuyProduct = document.getElementById('app-buy-product');
    const cartLists = document.getElementById('carts-list');
    console.log("enter back to cart");
    if (appBuyProduct && cartLists){
      appBuyProduct.classList.add('hide');
      cartLists.classList.remove('hide');
    }
  }
}

import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { SelectAddressComponent } from "../../../../shares/reusable/select-address/select-address.component";
import { CartItemModel } from '../../models/cart-item-model';
import { CreateOrderModel } from '../../models/create-order-model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-buy-product',
  standalone: true,
  imports: [SelectAddressComponent, FormsModule],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.scss'
})
export class BuyProductComponent {
  @Input() cartItems: CartItemModel[] = [];
  @Input() createOrderModel: CreateOrderModel = {
    email: '',
    phone: '',
    address: '',
    cartItems: []
  };

  @Output() getPersonalInfo = new EventEmitter<CreateOrderModel>();

  getPersonalInfoEvent(){
    this.getPersonalInfo.emit(this.createOrderModel);
  }

  getAddress(address: string){
    this.createOrderModel.address = address;
  }

}

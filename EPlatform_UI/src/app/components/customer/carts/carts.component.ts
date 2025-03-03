import { Component } from '@angular/core';
import { BuyProductComponent } from './buy-product/buy-product.component';

@Component({
  selector: 'app-carts',
  standalone: true,
  imports: [BuyProductComponent],
  templateUrl: './carts.component.html',
  styleUrl: './carts.component.scss'
})
export class CartsComponent {
  tickTheBox(event: Event) {
      var checkSpanElement = (event.currentTarget as HTMLElement).firstChild as HTMLElement;
      checkSpanElement.classList.toggle('checked');
  }

  minusQuantity(){

  }

  plusQuantity(){

  }
}

import { Component } from '@angular/core';
import { SelectAddressComponent } from "../../../../shares/reusable/select-address/select-address.component";

@Component({
  selector: 'app-buy-product',
  standalone: true,
  imports: [SelectAddressComponent],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.scss'
})
export class BuyProductComponent {

}

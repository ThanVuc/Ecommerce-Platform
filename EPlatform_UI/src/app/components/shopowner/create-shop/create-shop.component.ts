import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { ShopService } from '../../services/shop.service';
import { SelectAddressComponent } from "../../../shares/reusable/select-address/select-address.component";
import { CreateShopModel } from '../models/create-shop-model';
import { FormsModule } from '@angular/forms';
import { PhoneDirective } from '../../validator/PhoneValidator';

@Component({
  selector: 'app-create-shop',
  standalone: true,
  imports: [SelectAddressComponent, FormsModule, PhoneDirective],
  templateUrl: './create-shop.component.html',
  styleUrl: './create-shop.component.scss'
})
export class CreateShopComponent implements OnInit {
  ngOnInit(): void {
    this.renderer2.setStyle(this.document.body, 'background-color', '#0C0C0F');
    this.getUserId();
  }
  renderer2 = inject(Renderer2);
  document = inject(DOCUMENT);
  shopSVC = inject(ShopService);
  createShopModel: CreateShopModel = {
    ShopId: "",
    Name: "",
    Description: "",
    LogoImage: null,
    ShopAddress: "",
    Phone: "",
    Email: "",
    TaxesCode: "",
    IdentificationNumber: ""
  } 

  showForm(event: Event){
    const targetElemenet = event.target as HTMLElement;
    const welcomeElement = targetElemenet.parentElement;
    const formElement = welcomeElement?.nextElementSibling;
    if (formElement && welcomeElement) {
      formElement.classList.add("show");
      welcomeElement.classList.remove("show");
    }
  }

  getUserId(){
    this.shopSVC.getUserId().subscribe({
      next: (res) => {
        this.createShopModel.ShopId = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  saveAddress(address: string){
    console.log(address);
    this.createShopModel.ShopAddress = address;
  }

  onSubmit(event: Event){
    event.preventDefault();
  }

  saveFile(event: Event){
    event.preventDefault();
    const file =  (event.target as HTMLInputElement).files?.item(0);
    if (file){
      this.createShopModel.LogoImage = file;
    }
    console.log(this.createShopModel);
  }

  saveShopInfo(event: Event){
    const formEle = (event.target as HTMLElement).parentElement?.parentElement
    const congratulationEle = formEle?.nextElementSibling;

    if (congratulationEle && formEle){
      congratulationEle.classList.add("show");
      formEle.classList.remove("show");
    }

  }

  createShop(){
    this.shopSVC.createShop(this.createShopModel).subscribe({
      next: (res) => {
        console.log(res);
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  backToShopInfo(event: Event){
    const targetEle = (event.target as HTMLElement).parentElement;
    const formEle = targetEle?.previousElementSibling;

    if (formEle && targetEle){
      formEle.classList.add("show");
      targetEle.classList.remove("show");
    }

  }
}

import { Component, EventEmitter, inject, OnInit, Output, output } from '@angular/core';
import { SelectTagComponent } from "../select-tag/select-tag.component";
import { LocationModel } from '../../../components/models/location-model';
import { UtilitiesServiceService } from '../../../components/services/utilities-service.service';

@Component({
  selector: 'app-select-address',
  standalone: true,
  imports: [],
  templateUrl: './select-address.component.html',
  styleUrl: './select-address.component.scss'
})
export class SelectAddressComponent implements OnInit {
  ngOnInit(): void {
  }

  @Output() getAddress = new EventEmitter<string>();

  provinces: LocationModel[] = [];
  districts: LocationModel[] = [];
  wards: LocationModel[] = [];
  province: string = "";
  district: string = "";
  ward: string = "";
  detail: string = "";
  utilitiesSVC = inject(UtilitiesServiceService);

  showBoard(){
    this.getProvinces();
    const boards = document.querySelectorAll(".dropdown-board")
    .forEach(board => {
      board?.classList.add("show");
    });
  }

  hideBoard(){
    const boards = document.querySelectorAll(".dropdown-board")
    .forEach(board => {
      board?.classList.remove("show");
    });
  }

  choseProvince(event: Event ,code: string, province: string){
    const targetEle = event.target as HTMLElement;
    const parentEle = targetEle.parentElement;
    const provinceEle = document.getElementById("province") as HTMLInputElement;
    provinceEle.value = province;
    parentEle?.querySelectorAll("li").forEach(li => {
      li.classList.remove("selected");
    });
    targetEle.classList.add("selected");
    this.province = province;
    this.districts = [];
    this.wards = [];
    this.getDistricts(code);
  }

  choseDistrict(event: Event, code: string, district: string){
    const targetEle = event.target as HTMLElement;
    const parentEle = targetEle.parentElement;
    const districtEle = document.getElementById("district") as HTMLInputElement;
    districtEle.value = district;
    parentEle?.querySelectorAll("li").forEach(li => {
      li.classList.remove("selected");
    });
    targetEle.classList.add("selected");
    this.district += district;
    this.wards = [];
    this.getWards(code);
  }

  choseWards(event: Event, ward: string){
    const targetEle = event.target as HTMLElement;
    const parentEle = targetEle.parentElement;
    const wardEle = document.getElementById("ward") as HTMLInputElement;
    wardEle.value = ward;
    parentEle?.querySelectorAll("li").forEach(li => {
      li.classList.remove("selected");
    });
    targetEle.classList.add("selected");
    this.ward = ward;
    this.hideBoard();
  }

  getProvinces(){
    this.utilitiesSVC.getProvices().subscribe({
      next: (res) => {
        this.provinces = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  getDistricts(provinceCode: string){
    this.utilitiesSVC.getDistricts(provinceCode).subscribe({
      next: (res) => {
        this.districts = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  getWards(districtCode: string){
    this.utilitiesSVC.getWards(districtCode).subscribe({
      next: (res) => {
        this.wards = res.data;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  typeDetailAddress(event: Event){
    const targetEle = event.target as HTMLInputElement;
    this.detail = targetEle.value;
    this.saveAddress();
  }

  saveAddress(){
    this.getAddress.emit(`${this.detail}, ${this.ward}, ${this.district}, ${this.province}`);
  }

}

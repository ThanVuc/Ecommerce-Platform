import { DOCUMENT } from '@angular/common';
import { inject, Injectable, OnInit } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  document = inject(DOCUMENT)
  localStorage = this.document.defaultView?.localStorage; 
  constructor() {

  }
  setValue(key: string,value: string){
    this.localStorage?.setItem(key,value);
  }
  getValue(key: string){
    return this.localStorage?.getItem(key);
  }
  isExistKey(key: string) : boolean{
    if (this.localStorage?.getItem(key) == null){
      return false;
    }
    return true;
  }
  removeValue(key: string){
    this.localStorage?.removeItem(key);
  }
}

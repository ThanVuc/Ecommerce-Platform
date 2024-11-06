import { DOCUMENT } from '@angular/common';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  document = inject(DOCUMENT)
  sessionStorage = this.document.defaultView?.sessionStorage; 
  constructor() {

  }
  setValue(key: string,value: string){
    this.sessionStorage?.setItem(key,value);
  }
  getValue(key: string){
    return this.sessionStorage?.getItem(key);
  }
  isExistKey(key: string) : boolean{
    if (this.sessionStorage?.getItem(key) == null){
      return false;
    }
    return true;
  }
}

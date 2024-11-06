import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PassDataService {
  private titleSource = new BehaviorSubject<string>('');
  currentTitle = this.titleSource.asObservable();
  constructor() { }
  changeTitle(title: string){
    this.titleSource.next(title);
  }
}

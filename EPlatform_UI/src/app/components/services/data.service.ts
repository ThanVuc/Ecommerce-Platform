import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { StatusModel } from '../shopowner/orders/models/status-model';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  constructor() { }
  private messageSource = new BehaviorSubject<StatusModel[]>([]);
  private statusIdSource = new BehaviorSubject<number|null>(null);
  currentMessage = this.messageSource.asObservable();
  currentStatusId = this.statusIdSource.asObservable();

  changeMessage(message: StatusModel[]) {
    this.messageSource.next(message);
  }

  changeStatusId(statusId: number|null) {
    this.statusIdSource.next(statusId);
  }
}

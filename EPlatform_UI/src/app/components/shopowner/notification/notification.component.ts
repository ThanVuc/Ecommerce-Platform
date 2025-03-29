import { Component, inject, Input, OnInit } from '@angular/core';
import { SignalRService } from '../../services/signal-r.service';
import { NotificationModel } from '../models/notification-model';
import { ShopService } from '../../services/shop.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent implements OnInit {
  signalSVC = inject(SignalRService);
  shopSVC = inject(ShopService);
  notifications: NotificationModel[] = [];
  notificationCount: number = 0;
  activatedRoute = inject(ActivatedRoute);
  document = inject(DOCUMENT);
  @Input() shopId: string = "";

  ngOnInit(): void {
    this.loadNotifications();
    this.notificationCount = this.notifications.length;
  }
  
  
  loadNotifications() {
    this.shopSVC.getNotifications(this.shopId).subscribe(res => {
        if (res) {
          this.notifications = res.data;
          this.notificationCount = this.notifications.length;
        }
      }
    );
  }

  show(event: Event){
    event.preventDefault();
    event.stopPropagation();
    this.loadNotifications();
    const sideBarElement = event.target as HTMLElement;
    const notificationFrame = sideBarElement.nextElementSibling as HTMLElement;
    if (!notificationFrame) return;
    if (notificationFrame.classList.contains('show')) {
      notificationFrame.classList.remove('show');
      sideBarElement.classList.remove('active');
      return;
    };
    notificationFrame.classList.add('show');
    sideBarElement.classList.add('active');
  }

  hide(event: Event){
    event.preventDefault();
    event.stopPropagation();
    const notificationFrame = (event.target as HTMLElement).parentElement?.parentElement as HTMLElement;
    const sideBarElement = notificationFrame.previousElementSibling as HTMLElement;
    if (!notificationFrame) return;
    notificationFrame.classList.remove('show');
    sideBarElement.classList.remove('active');
  }

  remove(event: Event, notificationId: string) {
    event.preventDefault();
    event.stopPropagation();

    this.shopSVC.removeNotification(this.shopId , notificationId).subscribe(res => {
      if (res) {
        const li = (event.target as HTMLElement )?.parentElement as HTMLElement;
        li.classList.add('disapear');
        setTimeout(() => {
          (event.target as HTMLElement )?.parentElement?.remove();
          this.notificationCount -= 1;
          li.remove();
        },450);
      }
    });

  }
}

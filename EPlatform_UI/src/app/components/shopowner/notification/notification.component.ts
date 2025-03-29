import { Component } from '@angular/core';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent {
  show(event: Event){
    event.preventDefault();
    event.stopPropagation();
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
}

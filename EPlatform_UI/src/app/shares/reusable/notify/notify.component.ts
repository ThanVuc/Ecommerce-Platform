import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-notify',
  standalone: true,
  imports: [],
  templateUrl: './notify.component.html',
  styleUrl: './notify.component.scss'
})
export class NotifyComponent {
  @Input() notify: string = "No notification available.";
  showNotify(message: string){
    const notificationElement = document.querySelector('.notify') as HTMLElement;
    notificationElement.classList.add('fadeIn');
    setTimeout(() => {
      notificationElement.classList.remove('fadeIn');
    }, 3000);
  }
}

import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NotificationComponent } from "../../../components/shopowner/notification/notification.component";
import { NotifyComponent } from "../../reusable/notify/notify.component";
import { SignalRService } from '../../../components/services/signal-r.service';

@Component({
  selector: 'app-shopownerlayout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NotificationComponent, NotifyComponent],
  templateUrl: './shopownerlayout.component.html',
  styleUrl: './shopownerlayout.component.scss'
})
export class ShopownerlayoutComponent implements OnInit {
  @ViewChild(NotifyComponent) notifier!: NotifyComponent;
  @ViewChild(NotificationComponent) notification!: NotificationComponent;
  shop_id: string = "";
  activatedRouter = inject(ActivatedRoute);
  message: string = "";
  signalRService = inject(SignalRService);

  ngOnInit(): void {
    this.activatedRouter.params.subscribe(param => {
      this.shop_id = param["shop_id"];
    })

    this.signalRService.notification$.subscribe((message: string) => {
      this.message = message;
      this.notifier.showNotify(this.message);
      this.notification.notificationCount += 1;
    });
  }

  dropDown(event: Event){
    const btnElement = event.target as HTMLElement;
    var parent_id = btnElement.parentElement?.parentElement?.id;
    var drop_items = document.querySelectorAll(`#${parent_id} .dropdown-item`);
    drop_items.forEach(e => {
      if (e.classList.contains("show-item")){
        e.classList.remove('show-item');
      } else {
        e.classList.add('show-item');  
      }
    });
  }

  showSideBar(event: Event){
    document.querySelector('.sidebar')?.classList.toggle('show-sidebar');
    document.querySelector('.black-curtain')?.classList.toggle('show-sidebar');
    const btnElement = event.target as HTMLElement;
    btnElement.classList.toggle('hide-toggle');
  }

  hideSidebar(){
    document.querySelector('.sidebar')?.classList.remove('show-sidebar');
    document.querySelector('.black-curtain')?.classList.remove('show-sidebar');
  }
}

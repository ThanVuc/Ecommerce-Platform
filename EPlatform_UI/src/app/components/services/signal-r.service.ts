import { inject, Inject, Injectable, OnInit, PLATFORM_ID } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { start } from 'repl';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection!: signalR.HubConnection;
  private notificationSubject = new BehaviorSubject<string>("");
  notification$ = this.notificationSubject.asObservable();
  document = inject(DOCUMENT);

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.startConnection();
  }

  startConnection() {
    if (isPlatformBrowser(this.platformId)) {
      this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5119/notificationHub', {
        accessTokenFactory: () => localStorage.getItem('AccessToken') || '',
        transport: HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection
        .start()
        .then(() => console.log('SignalR Connected'))
        .catch((err) => console.error('Error connecting to SignalR:', err));

        this.hubConnection.on('ReceiveNotification', (message: string) => {
          this.notificationSubject.next(message);
        });
    }
  }

  sendNotification(userIds: string[], message: string) {
    if (!this.hubConnection || this.hubConnection.state !== HubConnectionState.Connected) {
      console.error('Hub connection is not established.');
      return;
    }
    this.hubConnection.invoke('SendNotification', userIds, message)
      .catch((err) => console.error('Error invoking SendNotification:', err));
  }

}

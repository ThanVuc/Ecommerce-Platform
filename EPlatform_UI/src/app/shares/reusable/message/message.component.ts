import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [],
  templateUrl: './message.component.html',
  styleUrl: './message.component.scss'
})
export class MessageComponent {
  showModal(status: string, message: string){
    let messageBoxElement = document.getElementById("message-box");
    let messageIcon = document.getElementById("message-icon");
    let messageText = document.getElementById("message-text");
    if (messageBoxElement && messageIcon && messageText){
      status == "success" ? messageIcon.className = "ooui--success" : messageIcon.className = "icon-park-outline--doc-fail";
      messageBoxElement.className = "message-box " + status;
      messageText.innerText = message;
      messageBoxElement.style.display = "flex";
      messageBoxElement.style.animationPlayState = "running";

      setTimeout(() => {
        messageBoxElement.style.display = 'none';  
      }, 600);
    }
  }

  closeModal(){
    let closeElement = document.getElementById("close-message");
    if (closeElement){
      let messageElement = document.getElementById("message-box");
      if (messageElement){
        messageElement.style.display = "none";
      }
    }
  }
}

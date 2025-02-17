import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import e from 'express';
import { selectModel } from '../common-model/select-model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-select-tag',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './select-tag.component.html',
  styleUrl: './select-tag.component.scss'
})
export class SelectTagComponent {
  @Output() select = new EventEmitter<selectModel>();
  @Input() items: selectModel[] = [];

  selectValue: string = '';
  document = inject(DOCUMENT);

  remToPx(rem: number): number {
    const rootFontSize = parseFloat(getComputedStyle(document.documentElement).fontSize);
    return rem * rootFontSize;
  }


  showSelectBoard(event: Event) {
    const targetElement = event.target as HTMLElement;
    const boardElement = targetElement.nextElementSibling as HTMLElement;
    let selectRect = targetElement.getBoundingClientRect();
    boardElement.style.display = 'block';
    let boardHeight = this.getHiddenElementHeight(boardElement);
    if (window.innerHeight - selectRect.bottom < boardHeight) {
      boardElement.style.top = `-${boardHeight-this.remToPx(1.5)}px`;
    } else {
      boardElement.style.top = "";
    }
    boardElement.style.display = 'none';
    

    boardElement.classList.toggle('active');
  }

  getHiddenElementHeight(el: HTMLElement): number {
    if (!el) return 0;

    // Store original styles
    const originalDisplay = el.style.display;
    const originalVisibility = el.style.visibility;
    const originalPosition = el.style.position;

    // Temporarily make the element visible
    el.style.display = "block";  // Ensure it's in the flow
    el.style.visibility = "hidden"; // Prevent flashing
    el.style.position = "absolute"; // Avoid layout shifts

    const height = el.offsetHeight; // Get computed height

    // Restore original styles
    el.style.display = originalDisplay;
    el.style.visibility = originalVisibility;
    el.style.position = originalPosition;

    return height;
}

  setValue(value: selectModel) {
    this.select.emit(value);
    this.selectValue = value.name;
    const boardElement = this.document.getElementById("dropdown-board") as HTMLElement;
    boardElement.classList.remove('active');
  }

  setValueFromParent(id: string){
    const name = this.items.find(item => item.id == id)?.name;
    if (name){
      this.selectValue = name;
    }
  }
}

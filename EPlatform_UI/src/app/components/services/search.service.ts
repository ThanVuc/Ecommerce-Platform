import { DOCUMENT } from '@angular/common';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  constructor() { }

  timer!: NodeJS.Timeout | null;
  document = inject(DOCUMENT);
  suggestionsId = "";
  searchInputId = "";

  showSuggestions() {
    const suggestions = this.document.getElementById("suggestions");
    const searchInput = this.document.getElementById("search-input");

    if (suggestions && searchInput ) {
      suggestions.style.display = "block";

      this.document.addEventListener("click", (event) => {
        if (!suggestions?.contains(event.target as Node) && searchInput !== suggestions) {
          this.hideSuggestions();
        }
      });
    }
  }

  hideSuggestions() {
    const suggestions = this.document.getElementById(this.suggestionsId);
    if (suggestions) {
      suggestions.style.display = 'none';
    }
  }
}

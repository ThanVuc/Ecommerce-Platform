import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-adminlayout',
  standalone: true,
  imports: [RouterModule,RouterOutlet,RouterLink],
  templateUrl: './adminlayout.component.html',
  styleUrl: './adminlayout.component.scss'
})
export class AdminlayoutComponent{
  http = inject(HttpClient);
  constructor(){
    this.http.get("http://localhost:5119/api/admin").subscribe((res) => {

    });
  }
}

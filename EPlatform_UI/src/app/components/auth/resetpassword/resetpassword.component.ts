import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-resetpassword',
  standalone: true,
  imports: [],
  templateUrl: './resetpassword.component.html',
  styleUrl: './resetpassword.component.scss'
})
export class ResetpasswordComponent implements OnInit {
  http = inject(HttpClient)
  ngOnInit(): void {
    this.http.get("http://localhost:5119/api/admin").subscribe((res) => {

    });
  }
}

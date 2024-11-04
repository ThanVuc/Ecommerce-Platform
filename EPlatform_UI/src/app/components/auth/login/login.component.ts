import { Component, inject, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  titleService: Title = inject(Title)
  constructor(){

  }
  ngOnInit(): void {
    this.titleService.setTitle("Login")
  }
}

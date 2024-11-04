import { Component, Inject, inject, OnInit } from '@angular/core';
import { Router } from 'express';
import { routes } from '../layout.routes';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { DOCUMENT } from '@angular/common';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-authlayout',
  standalone: true,
  imports: [RouterModule,RouterOutlet,RouterLink],
  templateUrl: './authlayout.component.html',
  styleUrl: './authlayout.component.scss'
})
export class AuthlayoutComponent implements OnInit {
  document: Document = inject(DOCUMENT)
  constructor(){
    
  }
  ngOnInit(): void {
    this.document.body.style.cssText = `
      background-color: #0d0d0d;
        color: #ffffff;
        font-family: Arial, sans-serif;
        margin: 0;
        padding: 0;
    `;
  }
}

import { Component, Inject, inject, Input, input, OnInit } from '@angular/core';
import { Router } from 'express';
import { routes } from '../layout.routes';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { DOCUMENT } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { PassDataService } from '../../../components/services/pass-data.service';

@Component({
  selector: 'app-authlayout',
  standalone: true,
  imports: [RouterModule,RouterOutlet,RouterLink],
  templateUrl: './authlayout.component.html',
  styleUrl: './authlayout.component.scss'
})
export class AuthlayoutComponent {
  constructor(){
    
  }
}

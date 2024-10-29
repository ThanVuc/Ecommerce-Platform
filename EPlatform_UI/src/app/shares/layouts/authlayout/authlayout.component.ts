import { Component } from '@angular/core';
import { Router } from 'express';
import { routes } from '../layout.routes';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-authlayout',
  standalone: true,
  imports: [RouterModule,RouterOutlet,RouterLink],
  templateUrl: './authlayout.component.html',
  styleUrl: './authlayout.component.scss'
})
export class AuthlayoutComponent {

}

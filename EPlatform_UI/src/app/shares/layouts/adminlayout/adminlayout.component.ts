import { Component } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-adminlayout',
  standalone: true,
  imports: [RouterModule,RouterOutlet,RouterLink],
  templateUrl: './adminlayout.component.html',
  styleUrl: './adminlayout.component.scss'
})
export class AdminlayoutComponent {

}

import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { AdminlayoutComponent } from "../../../shares/layouts/adminlayout/adminlayout.component";
import { AuthHeaderComponent } from "../../../shares/reusable/auth-header/auth-header.component";
import { Title } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-resetpassword',
  standalone: true,
  imports: [FormsModule, RouterLink, AuthHeaderComponent],
  templateUrl: './resetpassword.component.html',
  styleUrl: './resetpassword.component.scss'
})
export class ResetpasswordComponent implements OnInit {
  http = inject(HttpClient);
  titleService = inject(Title);
  title = "Reset Password";
  extraErr = "";
  ngOnInit(): void {
    this.titleService.setTitle(this.title);
  }

  resetPassword(){
    
  }
}

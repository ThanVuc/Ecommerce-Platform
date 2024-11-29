import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateUserModel } from '../../models/users/create-user-model';
import { DOCUMENT } from '@angular/common';
import { emit } from 'node:process';

@Component({
  selector: 'app-create-user',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-user.component.html',
  styleUrl: './create-user.component.scss'
})
export class CreateUserComponent {
  errorList: string[] = [];
  createUserModel: CreateUserModel = {
    address: "",
    confirmPassword: "",
    first: "",
    last: "",
    password: "",
    phoneNumber: "",
    username: ""
  };
  document = inject(DOCUMENT);
  @Output() create = new EventEmitter<CreateUserModel>();

  showCreateForm(){
    var createForm = this.document.getElementById("create-form");
    if (createForm){
      createForm.style.display = 'block';
    }
  }

  createUser(){
    this.create.emit(this.createUserModel);
    this.hideCreateForm();
  }

  hideCreateForm(){
    var createForm = this.document.getElementById("create-form");
    if (createForm){
      createForm.style.display = 'none';
    }
  }

  showError(){
    console.log("Show Err is run");
    var errorsDOM = this.document.getElementById("error-modal");
    if (errorsDOM){
      errorsDOM.style.display = 'block';
    }
  }


}

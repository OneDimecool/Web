import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { UserService } from "./user.service";
import { NgForm } from "@angular/forms";

@Component({
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {
  formLoginModel = {
    Email: '',
    Password:''
  }


  constructor(private userService: UserService, private router: Router) { }

  ngOnInit() {
    if (localStorage.getItem('token') != null) {
      this.router.navigateByUrl('/');
    }
  }

  login(form: NgForm) {
      this.userService.login(form.value)
  }
}





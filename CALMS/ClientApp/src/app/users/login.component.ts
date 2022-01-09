import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { UserService } from "./user.service";
import { NgForm } from "@angular/forms";

@Component({
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {
  formLoginModel = {
    UserName: '',
    Password:''
  }


  constructor(private userService: UserService, private router: Router) { }

  ngOnInit() {
    if (this.userService.authorizedUser()) {
      this.router.navigateByUrl('/');
    }
  }

  login(form: NgForm) {
      this.userService.login(form.value)
  }
}





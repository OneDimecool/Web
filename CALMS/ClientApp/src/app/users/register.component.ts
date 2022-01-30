import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { UserService } from "./user.service";


@Component({
  templateUrl: './register.component.html'
})

export class RegisterComponent implements OnInit {
  constructor(public userService: UserService, private router: Router, private toastr: ToastrService) { }
  ngOnInit() {
    this.userService. formRegisterModel.reset();
  }
  register() {
    this.userService.register().subscribe(
      (res: any) => {
        if (res.Succeeded)
        {
          this.userService.formRegisterModel.reset();
          this.router.navigateByUrl('/users/login');
          this.toastr.success("Registed successfully");
        }
        else
        {
          res.errors.forEach(error => { console.log(error.description); });
        }
      }
    )
  }
}

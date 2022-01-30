import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { UserService } from "../users/user.service";
import { MenuService } from "./menu.service";

@Component({
  templateUrl: 'addRole.component.html'
})

export class AddRoleComponent {
  constructor(public menuService: MenuService, private router: Router, private toastr: ToastrService,public userService:UserService) { }
  ngOnInit() {
    this.menuService.formRoleModel.reset();
  }
  AddRole() {
    this.userService.createRole(this.menuService.formRoleModel.value.RoleName).subscribe(
      (res: any) => {
        if (res.Succeeded) {
          this.menuService.formRoleModel.reset();
          this.router.navigateByUrl('roles');
          this.toastr.success("User Added successfully");
        }
        else {
          res.errors.forEach(error => { console.log(error.description); });
        }
      }
    )
  }
}



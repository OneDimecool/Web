import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Role, User } from "src/app/users/user.model";
import { UserService } from "src/app/users/user.service";

@Component({
  templateUrl: 'edit.component.html'
})

export class RoleEditComponent implements OnInit {
  public userForm: FormGroup;
  public allRoles;
  public selectedRoles = [];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private service: UserService,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.service.getRoles().subscribe(res => {
      console.log(res)
      this.allRoles = res as Role;
      this.allRoles.forEach(role => this.selectedRoles.push({ 'Name': role.Name, 'Selected': false }));
    });

    this.userForm = this.formBuilder.group({
      Id: new FormControl(),
      UserName: [{ value: '', disabled: true }],
      Roles: [],
      RolesSelected: []
    });

    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.service.GetUsers(id).subscribe(res => {
      this.userForm.patchValue(res as User);
      this.selectedRoles.forEach((role, index, array) => {
        if (this.userForm.controls['Roles'].value.includes(role.Name)) {
          array[index].Selected = true;
        }
      });
    },
      (error => { console.log(error); }))
  }

  public error(control: string, error: string) {
    return this.userForm.controls[control].hasError(error);
  }
  public cancel() {
    this.router.navigateByUrl('/users');
  }

  public save(userFormValue) {
    if (this.userForm.valid) {
      const user: User = {
        Id: userFormValue.Id,
        UserName: userFormValue.UserName,
        Roles: userFormValue.RolesSelected
      };
      this.service.Put(user).subscribe(() => {
        this.router.navigateByUrl('/users');
      },
        (error => { console.log(error) }));
    }
  }
}

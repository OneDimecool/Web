import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { MenuModel, RoleModel,RoleMenuModel } from "./menu.model";
import { UserService } from "src/app/users/user.service";
import { MenuService } from "./menu.service";

@Component({
  templateUrl: 'edit.component.html'
})

export class RoleEditComponent implements OnInit {
  public roleForm: FormGroup;
  public allMenus;
  public selectedMenus = [];

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private UserService: UserService,
    private MenuService: MenuService,
    private formBuilder: FormBuilder) { }
  //初始化-获取菜单列表把菜单名字加入selectedMenus数组，被选择状态全部设为false
  ngOnInit() {
    this.MenuService.GetMenus().subscribe(res => {
      console.log(res)
      this.allMenus = res as MenuModel;
      this.allMenus.forEach(menu => this.selectedMenus.push({ 'Name': menu.MenuName, 'Selected': false }));
    });
    //定义表单模板
    this.roleForm = this.formBuilder.group({
      Id: new FormControl(),
      RoleName: [{ value: '', disabled: true }],
      MenuName: [],
      MenuSelected: []
    });
    //用id查允许访问的菜单列表，更新表单数值，根据id的菜单列表，标记selected
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    this.MenuService.GetMenusById(id).subscribe(res => {
      this.roleForm.patchValue(res as RoleModel);
      this.selectedMenus.forEach((menu, index, array) => {
        if (this.roleForm.controls['Menus'].value.includes(menu.Name)) {
          array[index].Selected = true;
        }
      });
    },
      (error => { console.log(error); }))
  }

  public error(control: string, error: string) {
    return this.roleForm.controls[control].hasError(error);
  }
  public cancel() {
    this.router.navigateByUrl('/users');
  }

    public save(roleFormValue) {
    if (this.roleForm.valid) {
      const roleMenu: RoleMenuModel = {
        IdentityRoleId: roleFormValue.Id,
        ApplicationMenuId : roleFormValue.RoleName,
        Menu: userFormValue.RolesSelected
      };
      this.service.Put(user).subscribe(() => {
        this.router.navigateByUrl('/users');
      },
        (error => { console.log(error) }));
    }
  }
}

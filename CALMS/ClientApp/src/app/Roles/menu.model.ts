export class MenuModel {
  Id: string;
  MenuName: string;
}

export class RoleModel {
  Id: string;
  RoleName:string;
  MenuName: string[];
  MenuId: string[];
}

export class RoleMenuModel {
  IdentityRoleId: string;
  ApplicationMenuId:string[]
}

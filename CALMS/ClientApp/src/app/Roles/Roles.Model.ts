import { FormBuilder, Validators } from "@angular/forms";


export class RoleModel {
  constructor(private formBuiler: FormBuilder) { }
  formRoleModel = this.formBuiler.group({
    RoleName: ['',[Validators.required]]
  })
}

import { Inject, Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { FormBuilder,FormGroup,Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { Role } from "./user.model";

@Injectable({
  providedIn: 'root'
})

export class UserService {
  baseUrl: string;
  apiUrl = 'api/Users/';
  authorizedUser$: Subject<AuthorizedUser> = new Subject < AuthorizedUser > ();

  constructor(private formBuilder: FormBuilder, private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string,private router: Router,private toastr: ToastrService) {
    this.baseUrl = baseUrl;
  }

  formRegisterModel = this.formBuilder.group
    ({
      UserName: [''],
      Passwords: this.formBuilder.group(
        {
          Password: ['', [Validators.required, Validators.minLength(4)]],
          ConfirmPassword:['',Validators.required]
        },
        {
          validator: this.comparePasswords
        }
      )
    });
  register() {
    const body = {
      UserName: this.formRegisterModel.value.UserName,
      Password: this.formRegisterModel.value.Passwords.Password
    };
    return this.httpClient.post(this.baseUrl + this.apiUrl + 'Register', body);
  }

  createRole(role) {
    return this.httpClient.post(this.baseUrl + this.apiUrl + 'AddRole',role);
  }

  deleteRole(role) {
    return this.httpClient.delete(this.baseUrl + this.apiUrl + 'DeleteRole', role);
  }

  login(user) {
    this.authorizedUser$.next(
      {
        UserName:''
    });
    this.httpClient.post(this.baseUrl + this.apiUrl + 'Login', user).subscribe(
      (res: any) => {
          this.authorizedUser$.next({
            UserName: JSON.parse(window.atob((res.token.split('.')[1]))).Name
          });
        if (res) { 
          localStorage.setItem('token', res.token);
          this.toastr.success('Login successfully')
          this.router.navigateByUrl('/');
        }
      },
      (err) => {
        this.toastr.error('Invaild User Name or Password', '', { timeOut: 3000 });
      }
    );
  }

  logout() {
    this.authorizedUser$.next(undefined);
    localStorage.removeItem('token');
    this.router.navigateByUrl('/');
  }

  authorizedUser() {
    if (localStorage.getItem('token') != null) {
      const expiry = (JSON.parse(atob(localStorage.getItem('token').split('.')[1]))).exp;
      return (Math.floor((new Date).getTime()/1000)) <= expiry
    }
    return localStorage.getItem('token') != null;
  }

  getAuthorizedUserInfo() {
    //const token = new HttpHeaders({ 'Authorization': 'Bearer' + localStorage.getItem('token') });
    //return this.httpClient.get(this.baseUrl + this.apiUrl + 'GetAuthorizedUserInfo', { headers: token });
    return this.httpClient.get(this.baseUrl + this.apiUrl + 'GetAuthorizedUserInfo');
  }

  getAuthorizedUserName() {
    if (localStorage.getItem('token')) {
      return JSON.parse(window.atob(localStorage.getItem('token').split('.')[1])).UserName;
    }
    else {
      return ' ';
    }
  }

  comparePasswords(formBuilder: FormGroup) {
    const confirmPassword = formBuilder.get('ConfirmPassword');
    if (confirmPassword.errors == null || 'passwordMismatch' in confirmPassword.errors) {
      if (formBuilder.get('Password').value != confirmPassword.value) {
        confirmPassword.setErrors({ passwordMismatch: true });
      }
      else {
        confirmPassword.setErrors(null);
      }
    }
  }
  allowedRole(allowedRoles) {
    let match = false
    const role = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1])).role;
    if (typeof (role) == 'string') {
      allowedRoles.forEach(element => {
        if (role == element) {
          match = true;
          return false;
        }
      });
     }
      else if (Array.isArray(role)) {
      if (allowedRoles.filter(element => role.includes(element)).length > 0){
        match = true;
        }
    }
    return match;
  }
  public GetUsers(Id?)
  {
    if (Id) {
      return this.httpClient.get(this.baseUrl + this.apiUrl + Id);
    }
    else
    {
      return this.httpClient.get(this.baseUrl + this.apiUrl);
    }
  }

  delete(Id) {
    return this.httpClient.delete(this.baseUrl + this.apiUrl + Id);
  }

  Put(user) {
    return this.httpClient.put(this.baseUrl + this.apiUrl + user.Id, user);
  }
  public getRoles() {
    return this.httpClient.get(this.baseUrl + this.apiUrl + 'GetRoles');
   }
}


export interface AuthorizedUser {
  UserName: string;
}

import { Inject, Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
@Injectable({ providedIn: "root" })

export class MenuService {
  baseUrl: string;
  apiUrl = 'api/Menus/';

  constructor(private formBuilder: FormBuilder, private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string, private router: Router, private toastr: ToastrService) {
    this.baseUrl = baseUrl;
  }

  public GetMenus()
  {
    return this.httpClient.get(this.baseUrl + this.apiUrl);
  }
}

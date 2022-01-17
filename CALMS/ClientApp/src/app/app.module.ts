import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AdministrationComponent } from './administration/administration.component';
import { AuthorizedGuard } from './authorize/authorize.guard';
import { AuthorizeInterceptor } from './authorize/authorize.interceptor';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { UserService } from './users/user.service';
import { RegisterComponent } from './users/register.component';
import { LoginComponent } from './users/login.component';
import { RoleListComponent } from './Roles/roleList.component'
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule} from '@angular/material/sidenav'
import 'hammerjs';
import { UsersListComponent } from './users/list.component';
import { UserIndexComponent } from './users/index.component'; 
import { UserDetailsComponent } from './users/details.component';
import { UserEditComponent } from './users/edit.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    RegisterComponent,
    LoginComponent,
    AdministrationComponent,
    UsersListComponent,
    UserIndexComponent,
    UserDetailsComponent,
    UserEditComponent,
    RoleListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'users/register', component: RegisterComponent },
      { path: 'users/login', component: LoginComponent },
      { path: 'administration', component: AdministrationComponent, canActivate: [AuthorizedGuard], data: { allowedRoles: ['Administrator'] , pageName : 'administration' } },
      { path: 'users', component: UserIndexComponent, canActivate: [AuthorizedGuard], data: { allowedRoles: ['Administrator'] , pageName:'users'} },
      { path: 'users/:id', component: UserDetailsComponent, canActivate: [AuthorizedGuard], data: { allowedRoles: ['Administrator'] , pageName: 'userDetail'} },
      { path: 'users/edit/:id', component: UserEditComponent, canActivate: [AuthorizedGuard], data: { allowedRoles: ['Administrator'], pageName: 'userEdit' } },
      { path: 'roles', component: RoleListComponent, canActivate: [AuthorizedGuard], data: { allowedRoles: ['Administrator'], pageName: 'role' } }
    ]),
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatCardModule,
    MatButtonModule,
    MatDividerModule,
    MatListModule,
    MatSidenavModule,
    ToastrModule.forRoot()
  ],
  providers: [UserService,
    { provide:HTTP_INTERCEPTORS,useClass: AuthorizeInterceptor,multi:true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

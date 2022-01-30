import { error } from "@angular/compiler/src/util";
import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { Role } from "src/app/users/user.model";
import { UserService } from "src/app/users/user.service";

@Component({
  selector: 'roleList',
  templateUrl: 'roleList.component.html'
})

export class RoleListComponent implements OnInit, AfterViewInit {
  columns: string[] = ['Name', 'Details-Edit-Delete'];
  dataSource = new MatTableDataSource<Role>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private userService: UserService) {
    this.dataSource.filterPredicate = (role: Role, filter: string) => {
      return role.Name.toLowerCase().includes(filter.toLowerCase());

    }
  }
  ngOnInit() {
    this.get();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public get() {
    this.userService.getRoles().subscribe(res => {
      this.dataSource.data = res as Role[];
    });
  }

  public filter(filter: string) {
    this.dataSource.filter = filter.trim().toLowerCase();
  }

  delete(roleId) {
    if (confirm('Are you sure to delete this role?')) {
      this.userService.deleteRole(roleId).subscribe(() => {
        this.get();
      },
        err => {
          console.log((err));
        })
    }
  }

  create(role) {
    this.userService.createRole(role).subscribe(() => {
      this.get();
    },
      err => {
        console.log((err));
      })
  }
}

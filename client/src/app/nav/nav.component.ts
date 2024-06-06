import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  checkLogin = false;

  constructor(
    private accountService: AccountService
  ) { }

  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: res => {
        console.log(res);
        this.checkLogin = true;
      },
      error: err => {
        console.log(err);
      }
    })
  }

  logout() {
    this.checkLogin = false;
  }
}

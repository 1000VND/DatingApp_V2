import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const AuthGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService)

  return accountService.currentUser$.pipe(
    map(user => {
      if (user) return true;
      else {
        toastr.error('You need login!');
        return false;
      }
    })
  )
};

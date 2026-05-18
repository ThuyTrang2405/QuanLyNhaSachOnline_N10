import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn && auth.vaiTro === 'QuanTri') {
    return true;
  }

  // Không phải Admin → về trang chủ
  router.navigate(['/']);
  return false;
};

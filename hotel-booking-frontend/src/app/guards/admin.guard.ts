import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Step 1: Prevent guests (not logged in at all)
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // Step 2: Ensure the user actually has the Admin role
  if (authService.isAdmin()) {
    return true; // Access granted!
  }

  // Step 3: Regular users exploring where they shouldn't go
  router.navigate(['/hotels']);
  return false;
};

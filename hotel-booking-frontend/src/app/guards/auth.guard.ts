import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  // Use Angular's modern inject() function inside functional guards
  const authService = inject(AuthService);
  const router = inject(Router);

  // Call the signal to check if they have a token/identity
  if (authService.isLoggedIn()) {
    return true; // Access granted!
  } else {
    // Access denied! Redirect to the login page
    router.navigate(['/login']);
    return false;
  }
};

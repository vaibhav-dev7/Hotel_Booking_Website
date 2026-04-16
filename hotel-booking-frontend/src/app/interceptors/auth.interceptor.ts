import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Grab the service without needing a class constructor
  const authService = inject(AuthService);
  const token = authService.getToken();

  // If the user has a JWT token stored natively in localStorage
  if (token) {
    // We cannot mutate the original request, so we clone it!
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    // Send the freshly stamped request to ASP.NET
    return next(clonedRequest);
  }

  // If they aren't logged in, just pass the exact request as it was typed
  return next(req);
};

import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();
  const org = authService.currentOrganization();

  let headers = req.headers;

  if (token && !req.url.includes('/api/v1/auth/login') && !req.url.includes('/api/v1/auth/register-organization')) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  if (org?.id) {
    headers = headers.set('X-Organization-Id', org.id);
  }

  const authReq = req.clone({ headers });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/api/v1/auth/login')) {
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};

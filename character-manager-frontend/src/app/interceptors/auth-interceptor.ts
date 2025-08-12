import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  let clonedReq = req;

  if (token) {
    clonedReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  
  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) =>{
      if (error.status === 401) {
        console.log("ERROR!!!!")
        const refreshToken = authService.getRefreshToken();
        const userId = authService.getCurrentUserId();

        if (refreshToken && userId) {
          return authService.refreshToken(refreshToken, userId).pipe(
            switchMap(() => {
              const newToken = authService.getToken();
              const newReq = req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              });
              return next(newReq);
            })
          );
        }
      }
      return throwError(() => error);
    })
  );
};

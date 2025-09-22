import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { selectAuthToken } from '../store/auth.selectors';
import { of, switchMap, take, withLatestFrom } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);

  return of(req).pipe(
    withLatestFrom(store.select(selectAuthToken).pipe(take(1))),
    switchMap(([request, token]) => {
      if (token) {
        request = request.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        });
      }
      return next(request);
    })
  );
};

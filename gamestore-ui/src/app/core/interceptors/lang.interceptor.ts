import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { map, switchMap, take } from 'rxjs';
import { selectLang } from '../../features/games/store/local.selectors';

export const langInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);

  return store.select(selectLang).pipe(
    take(1),
    map(lang => {
      if (lang) {
        req = req.clone({
          setHeaders: { 'Accept-Language': lang }
        });
      }
      return req;
    }),
    switchMap(updatedReq => next(updatedReq))
  );
};

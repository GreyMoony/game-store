import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { map, catchError, mergeMap } from 'rxjs/operators';
import * as AuthActions from './auth.actions';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthEffects {
  constructor(private readonly actions$: Actions, private readonly authService: AuthService) {}

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      mergeMap(() =>
        this.authService.login().pipe(
          map(res => AuthActions.loginSuccess({ token: res.token })),
          catchError(error => of(AuthActions.loginFailure({ error })))
        )
      )
    )
  );
}

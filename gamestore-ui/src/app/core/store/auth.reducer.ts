import { createReducer, on } from '@ngrx/store';
import * as AuthActions from './auth.actions';
import { AuthState } from './auth.models';

export const initialState: AuthState = {
  token: null,
  isAuthenticated: false
};

export const authReducer = createReducer(
  initialState,
  on(AuthActions.loginSuccess, (state, { token }) => ({
    ...state,
    token,
    isAuthenticated: true
  })),
  on(AuthActions.logout, state => ({
    ...state,
    token: null,
    isAuthenticated: false
  }))
);

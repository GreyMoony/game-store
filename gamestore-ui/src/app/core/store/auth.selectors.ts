import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.models';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectAuthToken = createSelector(
  selectAuthState,
  state => state.token
);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  state => state.isAuthenticated
);

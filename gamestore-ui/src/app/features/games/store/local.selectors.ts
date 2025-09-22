import { createFeatureSelector, createSelector } from '@ngrx/store';
import { LocalState } from './local.models';

export const selectLocalState = createFeatureSelector<LocalState>('local');

export const selectLang = createSelector(
  selectLocalState,
  state => state.lang
);

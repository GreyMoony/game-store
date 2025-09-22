import { createReducer, on } from '@ngrx/store';
import * as LocalActions from './local.actions';
import { LocalState } from './local.models';

export const initialState: LocalState = {
  lang: "eng",
};

export const localReducer = createReducer(
  initialState,
  on(LocalActions.localChange, (state, { lang }) => ({
    ...state,
    lang
  }))
);

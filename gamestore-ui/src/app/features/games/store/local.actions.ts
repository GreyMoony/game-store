import { createAction, props } from '@ngrx/store';

export const localChange = createAction(
  '[Local] Change Language',
  props<{ lang: string }>()
);

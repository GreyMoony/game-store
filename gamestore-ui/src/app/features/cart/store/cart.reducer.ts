import { createReducer, on } from '@ngrx/store';
import * as CartActions from './cart.actions';
import { CartState } from './cart.models';

export const initialState: CartState = {
  items: [],
  loading: false,
  error: null,
};

export const cartReducer = createReducer(
  initialState,
  on(CartActions.loadCart, state => ({ ...state, loading: true })),
  on(CartActions.loadCartSuccess, (state, { items }) => ({
    ...state,
    items,
    loading: false,
  })),
  on(CartActions.loadCartFailure, (state, { error }) => ({
    ...state,
    error,
    loading: false,
  }))
);

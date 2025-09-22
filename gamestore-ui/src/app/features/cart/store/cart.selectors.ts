import { createFeatureSelector, createSelector } from '@ngrx/store';
import { CartState } from './cart.models';

export const selectCartState = createFeatureSelector<CartState>('cart');

export const selectCartItems = createSelector(selectCartState, state => state.items);
export const selectCartCount = createSelector(selectCartItems, items =>
  items.reduce((sum, item) => sum + item.quantity, 0)
);
export const selectCartTotal = createSelector(selectCartItems, items =>
  items.reduce((sum, item) => sum + (item.price - (item.price * item.discount) / 100) * item.quantity, 0)
);

import { createAction, props } from '@ngrx/store';
import { CartItem } from './cart.models';

export const loadCart = createAction('[Cart] Load Cart');
export const loadCartSuccess = createAction('[Cart] Load Cart Success', props<{ items: CartItem[] }>());
export const loadCartFailure = createAction('[Cart] Load Cart Failure', props<{ error: any }>());

export const addToCart = createAction('[Cart] Add To Cart', props<{ gameKey: string }>());
export const addToCartFailure = createAction('[Cart] Add To Cart Failure', props<{ error: any }>());

export const removeFromCart = createAction('[Cart] Remove From Cart', props<{ gameKey: string }>());
export const removeFromCartFailure = createAction('[Cart] Remove From Cart Failure', props<{ error: any }>());

export const updateCartItemQuantity = createAction('[Cart] Update Cart Item Quantity', props<{ id: string, quantity: number }>());
export const updateCartItemQuantityFailure = createAction('[Cart] Update Cart Item Quantity Failure', props<{ error: any }>());

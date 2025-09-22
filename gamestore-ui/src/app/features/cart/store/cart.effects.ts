import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { CartService } from '../../../core/services/cart.service';
import * as CartActions from './cart.actions';
import { catchError, map, mergeMap, of } from 'rxjs';

@Injectable()
export class CartEffects {
  constructor(private readonly actions$: Actions, private readonly cartService: CartService) {}

  loadCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.loadCart),
      mergeMap(() =>
        this.cartService.getCart().pipe(
          map(items => CartActions.loadCartSuccess({ items })),
          catchError(error => of(CartActions.loadCartFailure({ error })))
        )
      )
    )
  );

  addToCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.addToCart),
      mergeMap(({ gameKey }) =>
        this.cartService.addToCart(gameKey).pipe(
          map(() => CartActions.loadCart()),
          catchError(error => of(CartActions.addToCartFailure({ error })))
        )
      )
    )
  );

  removeFromCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.removeFromCart),
      mergeMap(({ gameKey }) =>
        this.cartService.deleteFromCart(gameKey).pipe(
          map(() => CartActions.loadCart()),
          catchError(error => of(CartActions.removeFromCartFailure({ error })))
        )
      )
    )
  );

  updateCartItemQuantity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.updateCartItemQuantity),
      mergeMap(({ id, quantity }) =>
        this.cartService.changeQuantity(id, quantity).pipe(
          map(() => CartActions.loadCart()),
          catchError(error => of(CartActions.loadCartFailure({ error })))
        )
      )
    )
  );
}

import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { CartItem } from '../../../../core/services/cart.service';
import * as CartActions from '../../store/cart.actions';
import { selectCartCount, selectCartItems, selectCartTotal } from '../../store/cart.selectors';
import { GamesService } from '../../../../core/services/games.service';
import { CartItemComponent } from "../../components/cart-item/cart-item.component";
import { TAX_RATE } from '../../../../core/tokens/tax-rate.token';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, CartItemComponent, TranslationPipe],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss'
})
export class CartPageComponent implements OnInit {
  items$: Observable<CartItem[]>;
  subtotal$: Observable<number>;
  count$: Observable<number>;
  imageUrl: string = '';

  constructor(
    private readonly store: Store,
    private readonly gamesService: GamesService,
    @Inject(TAX_RATE) public taxRate: number) {
    this.items$ = this.store.select(selectCartItems);
    this.subtotal$ = this.store.select(selectCartTotal);
    this.count$ = this.store.select(selectCartCount);
  }

  ngOnInit(): void {
    this.store.dispatch(CartActions.loadCart());
  }

  calcTax(subtotal: number): number {
    return subtotal * this.taxRate;
  }

  calcTotal(subtotal: number): number {
    return subtotal + this.calcTax(subtotal);
  }
}


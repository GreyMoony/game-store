import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { CartItem } from '../../store/cart.models';
import { GameModel, GamesService } from '../../../../core/services/games.service';
import * as CartActions from '../../store/cart.actions';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [RouterLink, CommonModule, TranslationPipe],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent implements OnInit {
  @Input() item!: CartItem;
  game: GameModel | null = null;
  imageUrl: string = '';

  constructor(
    private readonly gameService: GamesService,
    private readonly store: Store
  ) { }

  ngOnInit(): void {
    this.gameService.getGameById(this.item.productId).subscribe(
      (game) => {
        this.game = game;
        this.gameService.getGameImage(this.game.key).subscribe(blob => {
          this.imageUrl = URL.createObjectURL(blob);
        });
      }
    );
  }

  discountedPrice() {
    return (this.item.price * (1 - this.item.discount / 100)).toFixed(2);
  }

  remove() {
    if (this.game) {
      this.store.dispatch(CartActions.removeFromCart({ gameKey: this.game.key }));
    }
  }

  increase() {
    if (this.game) {
      this.store.dispatch(CartActions.addToCart({ gameKey: this.game.key }));
    }
  }

  decrease() {
    if (this.item.quantity == 1) {
      if (this.game) {
        this.store.dispatch(CartActions.removeFromCart({ gameKey: this.game.key }));
      }
    }
    else if (this.item.quantity > 1){
      this.store.dispatch(CartActions.updateCartItemQuantity({ id: this.item.id, quantity: this.item.quantity - 1 }));
    }
  }
}

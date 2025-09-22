import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface CartItem {
  id: string;
  productId: string;
  price: number;
  quantity: number;
  discount: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly baseUrl = 'http://localhost:5090/api';

  constructor(private readonly http: HttpClient) { }

  getCart(): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.baseUrl}/orders/cart`);
  }

  addToCart(gameKey: string) {
    return this.http.post(`${this.baseUrl}/games/${gameKey}/buy`, {});
  }

  deleteFromCart(gameKey: string) {
    return this.http.delete(`${this.baseUrl}/orders/cart/${gameKey}`);
  }

  changeQuantity(id: string, quantity: number) {
    return this.http.patch(`${this.baseUrl}/orders/details/${id}/quantity`, { Count: quantity });
  }
}

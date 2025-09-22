export interface CartItem {
  id: string;
  productId: string;
  price: number;
  quantity: number;
  discount: number;
}

export interface CartState {
  items: CartItem[];
  loading: boolean;
  error: any;
}

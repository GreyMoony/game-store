import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from "./shared/components/footer/footer.component";
import { HeaderComponent } from './shared/components/header/header.component';
import { Store } from '@ngrx/store';
import { login } from './core/store/auth.actions';
import { loadCart } from './features/cart/store/cart.actions';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FooterComponent, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'GameStore';

  constructor(private readonly store: Store) {
    this.store.dispatch(login());
    this.store.dispatch(loadCart());
  }
}

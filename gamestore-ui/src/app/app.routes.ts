import { Routes } from '@angular/router';
import { HomePageComponent } from './features/home/home-page/home-page.component';
export const routes: Routes = [
  { path: '', component: HomePageComponent, title: 'Game Store' },
  {
    path: 'games',
    loadComponent: () => import('./features/games/pages/games-list/games-list.component')
      .then(m => m.GamesListComponent),
    title: 'Games Store - List'
  },
  {
    path: 'games/:key',
    loadComponent: () =>
    import('./features/games/pages/game-details/game-details.component')
      .then(m => m.GameDetailsComponent),
    title: 'Game Store - Game Details'
  },
  {
    path: 'cart',
    loadComponent: () =>
    import('./features/cart/pages/cart-page/cart-page.component')
      .then(m => m.CartPageComponent),
    title: 'Game Store - Your Cart'
  },
  { path: '**', redirectTo: '' }
];

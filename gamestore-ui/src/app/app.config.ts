import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthEffects } from './core/store/auth.effects';
import { provideEffects } from '@ngrx/effects';
import { provideStore, Store } from '@ngrx/store';
import { authReducer } from './core/store/auth.reducer';
import { CartEffects } from './features/cart/store/cart.effects';
import { cartReducer } from './features/cart/store/cart.reducer';
import { initializeApp } from './app.init';
import { TAX_RATE } from './core/tokens/tax-rate.token';
import { localReducer } from './features/games/store/local.reducer';
import { langInterceptor } from './core/interceptors/lang.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled',
      })
    ),
    provideHttpClient(withInterceptors([authInterceptor, langInterceptor])),
    provideStore({ auth: authReducer, cart: cartReducer, local: localReducer }),
    provideEffects([AuthEffects, CartEffects]),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [Store],
      multi: true
    },
    { provide: TAX_RATE, useValue: 0.2 }
  ]
};

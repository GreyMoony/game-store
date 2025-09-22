import { Store } from '@ngrx/store';
import { login } from './core/store/auth.actions';
import { selectAuthToken } from './core/store/auth.selectors';
import { filter, take } from 'rxjs/operators';

export function initializeApp(store: Store) {
  return () => {
    // Dispatch action to fetch token
    store.dispatch(login());

    // Return a promise that resolves when token is ready
    return store.select(selectAuthToken).pipe(
      filter(token => !!token), // wait until token exists
      take(1)
    ).toPromise();
  };
}

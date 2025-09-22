import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectCartCount } from '../../../features/cart/store/cart.selectors';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { localChange } from '../../../features/games/store/local.actions';
import { FormsModule } from '@angular/forms';
import { TranslationPipe } from '../../../core/pipes/translation.pipe';
import { TranslationService } from '../../../core/services/translation.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, TranslationPipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  cartCount$ = this.store.select(selectCartCount);
  navOpen = false;
  selectedLanguage = 'en';

  constructor(
    private readonly store: Store,
    private readonly router: Router,
    private readonly translationService: TranslationService) { }

  toggleNav() {
    this.navOpen = !this.navOpen;
  }

  closeNav() {
    this.navOpen = false;
  }

  changeLanguage(lang: string) {
    this.store.dispatch(localChange({ lang }));

    const currentUrl = this.router.url;
    this.translationService.setLanguage(lang);
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
    });
  }
}

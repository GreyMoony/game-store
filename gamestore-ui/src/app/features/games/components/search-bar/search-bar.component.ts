import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [FormsModule, TranslationPipe],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  private readonly searchInput$ = new Subject<string>();
  searchTerm = '';
  @Output() search = new EventEmitter<string>();

  constructor() {
    // debounce so we don’t spam API on every keystroke
    this.searchInput$
      .pipe(
        debounceTime(400),
        distinctUntilChanged()
      )
      .subscribe(value => this.search.emit(value.trim()));
  }

  onInputChange(value: string) {
    this.searchInput$.next(value);
  }
}

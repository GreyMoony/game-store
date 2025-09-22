import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs/internal/Observable';
import { GameDto, GameQueryModel, GamesService, PagedGames } from '../../../../core/services/games.service';
import { SearchBarComponent } from "../../components/search-bar/search-bar.component";
import { GameCardComponent } from '../../components/game-card/game-card.component';
import { GamesFiltersComponent } from '../../components/games-filters/games-filters.component';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-games-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SearchBarComponent,
    GameCardComponent,
    GamesFiltersComponent,
    TranslationPipe],
  templateUrl: './games-list.component.html',
  styleUrl: './games-list.component.scss'
})
export class GamesListComponent implements OnInit {
  games: GameDto[] = [];
  totalPages = 0;
  currentPage = 1;
  loading = false;
  showFilters = false;

  query: GameQueryModel = {
    genres: [],
    publishers: [],
    platforms: [],
    page: 1,
    pageCount: "10",
  }

  constructor(private readonly gamesService: GamesService) { }

  ngOnInit(): void {
    this.loadGames();
  }

  loadGames() {
    this.loading = true;
    this.gamesService.getGames(this.query).subscribe(res => {
      this.games = res.games;
      this.totalPages = res.totalPages;
      this.currentPage = res.currentPage;
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  onSearch(name: string) {
    this.query.name = name;
    this.query.page = 1;
    this.loadGames();
  }

  onFilterChange(filters: Partial<GameQueryModel>) {
    this.query = { ...this.query, ...filters, page: 1 };
    this.loadGames();
  }

  onPageChange(page: number | string) {
    if (typeof page === 'number') {
      this.query.page = page;
      this.loadGames();
    }
  }

  get pagesToDisplay(): (number | string)[] {
    const pages: (number | string)[] = [];

    // Always show first
    if (this.currentPage > 3) {
      pages.push(1);
      if (this.currentPage > 4) {
        pages.push('...');
      }
    }

    // Window of pages around current
    for (let i = this.currentPage - 2; i <= this.currentPage + 2; i++) {
      if (i > 0 && i <= this.totalPages) {
        pages.push(i);
      }
    }

    // Always show last
    if (this.currentPage < this.totalPages - 2) {
      if (this.currentPage < this.totalPages - 3) {
        pages.push('...');
      }
      pages.push(this.totalPages);
    }

    return pages;
  }
}

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { GenresService, ShortGenreModel } from '../../../../core/services/genres.service';
import { PlatformModel, PlatformsService } from '../../../../core/services/platforms.service';
import { PublisherModel, PublishersService } from '../../../../core/services/publishers.service';
import { GameQueryModel, GamesService } from '../../../../core/services/games.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-games-filters',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslationPipe],
  templateUrl: './games-filters.component.html',
  styleUrl: './games-filters.component.scss'
})
export class GamesFiltersComponent implements OnInit {
  genres: ShortGenreModel[] = [];
  platforms: PlatformModel[] = [];
  publishers: PublisherModel[] = [];
  sortingOptions: string[] = [];
  publishDateOptions: string[] = [];

  filters = {
    sort: '',
    minPrice: null as number | null,
    maxPrice: null as number | null,
    publishers: [] as string[],
    genres: [] as string[],
    platforms: [] as string[],
    datePublishing: ''
  };
  @Output() filtersChange = new EventEmitter<Partial<GameQueryModel>>();

  constructor(
    private readonly gamesService: GamesService,
    private readonly genresService: GenresService,
    private readonly platformsService: PlatformsService,
    private readonly publishersService: PublishersService
  ) { }

  ngOnInit(): void {
    this.genresService.getAll().subscribe(res => this.genres = res);
    this.platformsService.getAll().subscribe(res => this.platforms = res);
    this.publishersService.getAll().subscribe(res => this.publishers = res);
    this.gamesService.getSortingOptions().subscribe(res => this.sortingOptions = res);
    this.gamesService.getPublishDateOptions().subscribe(res => this.publishDateOptions = res);
  }

  onChange() {
    this.filtersChange.emit(this.filters);
  }

  onCheckboxChange(type: 'genres' | 'publishers' | 'platforms', event: Event) {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    this.filters[type] = this.filters[type] || [];
    const list = this.filters[type];

    if (input.checked) {
      if (!list.includes(value)) {
        list.push(value);
      }
    } else {
      const index = list.indexOf(value);
      if (index >= 0) {
        list.splice(index, 1);
      }
    }

    this.onChange();
  }
}

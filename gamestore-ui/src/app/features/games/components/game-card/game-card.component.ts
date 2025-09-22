import { Component, Input } from '@angular/core';
import { GameDto, GamesService } from '../../../../core/services/games.service';
import { CommonModule } from '@angular/common';
import { GenresService } from '../../../../core/services/genres.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-game-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './game-card.component.html',
  styleUrl: './game-card.component.scss'
})
export class GameCardComponent {
  @Input() game!: GameDto;

  imageUrl: string = '';
  genres: string[] = [];

  constructor(private readonly gamesService: GamesService, private readonly genresService: GenresService) {}

  ngOnInit(): void {
    this.loadImage();
    this.loadGenres();
  }

  private loadImage() {
    this.gamesService.getGameImage(this.game.key).subscribe(blob => {
      this.imageUrl = URL.createObjectURL(blob);
    });
  }

  private loadGenres() {
    this.genresService.getGameGenres(this.game.key).subscribe(genres => {
      this.genres = genres.map(genre => genre.name);
    });
  }
}

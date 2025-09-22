import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { GameModel, GamesService } from '../../../../core/services/games.service';
import { CommentDto, CommentsService } from '../../../../core/services/comments.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GenresService, ShortGenreModel } from '../../../../core/services/genres.service';
import { PlatformModel, PlatformsService } from '../../../../core/services/platforms.service';
import { PublishersService, PublisherModel } from '../../../../core/services/publishers.service';
import { Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import * as CartActions from '../../../cart/store/cart.actions';
import { TAX_RATE } from '../../../../core/tokens/tax-rate.token';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';
import { CommentThreadComponent } from '../../components/comment-thread/comment-thread.component';

@Component({
  selector: 'app-game-details',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslationPipe, CommentThreadComponent],
  templateUrl: './game-details.component.html',
  styleUrl: './game-details.component.scss'
})
export class GameDetailsComponent implements OnInit, OnDestroy {
  game!: GameModel;
  imageUrl = '';
  genres: ShortGenreModel[] = [];
  platforms: PlatformModel[] = [];
  publisher: PublisherModel | null = null;
  comments: CommentDto[] = [];
  totalComments = 0;
  rating = 4;
  likeState = new Map<string, boolean>();
  likeCounts = new Map<string, number>();

  loading = true;

  private readonly sub = new Subscription();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly gamesService: GamesService,
    private readonly genresService: GenresService,
    private readonly platformsService: PlatformsService,
    private readonly publishersService: PublishersService,
    private readonly commentsService: CommentsService,
    private readonly store: Store,
    @Inject(TAX_RATE) public taxRate: number
  ) { }

  ngOnInit(): void {
    const key = this.route.snapshot.paramMap.get('key')!;
    this.gamesService.getGameImage(key).subscribe(blob => {
      this.imageUrl = URL.createObjectURL(blob);
    });

    this.sub.add(this.gamesService.getGame(key).subscribe((data) => { this.game = data; this.loading = false; }));
    this.sub.add(this.genresService.getGameGenres(key).subscribe((data) => (this.genres = data)));
    this.sub.add(this.platformsService.getGamePlatforms(key).subscribe((data) => (this.platforms = data)));
    this.sub.add(this.publishersService.getGamePublisher(key).subscribe((data) => (this.publisher = data)));
    this.loadComments(key);
  }

  ngOnDestroy() { this.sub.unsubscribe(); }

  // ----- Price helpers -----
  get hasDiscount(): boolean {
    return !!this.game && this.game.discount > 0;
  }
  get oldPrice(): number {
    return this.game?.price ?? 0;
  }
  get discountedPrice(): number {
    if (!this.game) return 0;
    return +(this.game.price * (1 - this.game.discount / 100)).toFixed(2);
  }
  get tax(): number {
    // tax is applied to the final price (after discount)
    return +((this.hasDiscount ? this.discountedPrice : this.oldPrice) * this.taxRate).toFixed(2);
  }

  // ----- Likes (UI-only) -----
  toggleLike = (id: string) => {
    const current = this.likeState.get(id) ?? false;
    this.likeState.set(id, !current);
    const base = this.likeCounts.get(id) ?? 0;
    this.likeCounts.set(id, current ? Math.max(0, base - 1) : base + 1);
  };

  isLiked = (id: string) => this.likeState.get(id) === true;

  likesFor = (id: string) => this.likeCounts.get(id) ?? 0;

  // ----- Gallery -----
  // For now we only have one image endpoint; thumbnails reuse it.
  setMainImage(url: string) { this.imageUrl = url; }

  countAllComments(comments: CommentDto[]): number {
    let count = 0;

    for (const comment of comments) {
      count++; // count the parent comment
      if (comment.childComments && comment.childComments.length > 0) {
        count += this.countAllComments(comment.childComments); // recursively count children
      }
    }

    return count;
  }

  loadComments(key: string) {
    this.commentsService.getComments(key).subscribe((data) => {
      this.comments = data;
      this.totalComments = this.countAllComments(this.comments);
    });
  }

  addToCart() {
    this.store.dispatch(CartActions.addToCart({ gameKey: this.game.key }));
  }
}

import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommentDto } from '../../../../core/services/comments.service';
import { TranslationPipe } from '../../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-comment-thread',
  standalone: true,
  imports: [CommonModule, TranslationPipe],
  templateUrl: './comment-thread.component.html',
  styleUrl: './comment-thread.component.scss'
})
export class CommentThreadComponent {
  @Input() comment!: CommentDto;
  @Input() isLiked!: (id: string) => boolean;
  @Input() likesFor!: (id: string) => number;
  @Input() toggleLike!: (id: string) => void;
}

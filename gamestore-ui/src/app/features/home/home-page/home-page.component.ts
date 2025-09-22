import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslationPipe } from '../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink, TranslationPipe],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent {

}

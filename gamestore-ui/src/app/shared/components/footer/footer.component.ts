import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslationPipe } from '../../../core/pipes/translation.pipe';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink, TranslationPipe],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {

}

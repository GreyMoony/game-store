import { Pipe, PipeTransform } from '@angular/core';
import { TranslationService } from '../services/translation.service';

@Pipe({ name: 't', pure: false, standalone: true })
export class TranslationPipe implements PipeTransform {
  constructor(private readonly ts: TranslationService) {}
  transform(key: string): string {
    return this.ts.translate(key);
  }
}

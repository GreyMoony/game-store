import { Injectable } from '@angular/core';
import en from '../translations/en.json';
import uk from '../translations/uk.json';
import es from '../translations/es.json';

type Translations = typeof en;

@Injectable({ providedIn: 'root' })
export class TranslationService {
  private currentLang = 'en';
  private readonly translations: Record<string, Translations> = { en, uk, es };

  setLanguage(lang: string) {
    this.currentLang = lang;
  }

  translate(key: string): string {
    const parts = key.split('.');
    let value: any = this.translations[this.currentLang];
    for (const part of parts) {
      value = value?.[part];
      if (!value) break;
    }
    return value ?? key;
  }
}

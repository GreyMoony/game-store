import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface GameDto {
  id: string;
  name: string;
  key: string;
  description: string;
  price: number;
  unitInStock: number;
  discount: number;
}

export interface PagedGames {
  games: GameDto[];
  totalPages: number;
  currentPage: number;
}

export interface GameQueryModel {
  genres: string[];
  publishers: string[];
  platforms: string[];
  minPrice?: number | null;
  maxPrice?: number | null;
  name?: string | null;
  datePublishing?: string | null;
  sort?: string | null;
  page: number;
  pageCount: string;
  trigger?: string | null;
}

export interface GameModel {
  id: string;
  name: string;
  key: string;
  description: string;
  price: number;
  unitInStock: number;
  discount: number;
}

@Injectable({
  providedIn: 'root'
})
export class GamesService {
  private readonly baseUrl = 'http://localhost:5090/api/games';

  constructor(private readonly http: HttpClient) { }

  getGames(query: GameQueryModel): Observable<PagedGames> {
    let params = new HttpParams();

    Object.entries(query).forEach(([key, value]) => {
      if (Array.isArray(value)) {
        value.forEach(v => params = params.append(key, v));
      } else if (value !== null && value !== undefined) {
        params = params.set(key, value.toString());
      }
    });

    return this.http.get<PagedGames>(this.baseUrl, { params });
  }

  getSortingOptions(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/sorting-options`);
  }

  getPublishDateOptions(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/publish-date-options`);
  }

  getGameImage(key: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${key}/image`, { responseType: 'blob' });
  }

  getGame(key: string): Observable<GameModel> {
    return this.http.get<GameModel>(`${this.baseUrl}/${key}`);
  }

  getGameById(id: string): Observable<GameModel> {
    return this.http.get<GameModel>(`${this.baseUrl}/find/${id}`);
  }
}

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface ShortGenreModel {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class GenresService {
  private readonly baseUrl = 'http://localhost:5090/api/genres';

  constructor(private readonly http: HttpClient) { }

  getAll(): Observable<ShortGenreModel[]> {
    return this.http.get<ShortGenreModel[]>(this.baseUrl);
  }

  getGameGenres(key: string): Observable<ShortGenreModel[]> {
    return this.http.get<ShortGenreModel[]>(`http://localhost:5090/api/games/${key}/genres`);
  }
}

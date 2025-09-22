import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface PlatformModel {
  id: string;
  type: string;
}

@Injectable({
  providedIn: 'root'
})
export class PlatformsService {
  private readonly baseUrl = 'http://localhost:5090/api';

  constructor(private readonly http: HttpClient) { }

  getAll(): Observable<PlatformModel[]> {
    return this.http.get<PlatformModel[]>(`${this.baseUrl}/platforms`);
  }

  getGamePlatforms(gameKey: string): Observable<PlatformModel[]> {
    return this.http.get<PlatformModel[]>(`${this.baseUrl}/games/${gameKey}/platforms` );
  }
}

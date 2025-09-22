import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface PublisherModel {
  id: string;
  companyName: string;
  description?: string | null;
  homePage?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class PublishersService {
  private readonly baseUrl = 'http://localhost:5090/api';

  constructor(private readonly http: HttpClient) { }

  getAll(): Observable<PublisherModel[]> {
    return this.http.get<PublisherModel[]>(`${this.baseUrl}/publishers`);
  }

  getGamePublisher(gameKey: string): Observable<PublisherModel> {
    return this.http.get<PublisherModel>(`${this.baseUrl}/games/${gameKey}/publisher` );
  }
}

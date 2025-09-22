import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface CommentDto {
  id: string;
  name: string;
  body: string;
  childComments: CommentDto[];
}

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  private readonly baseUrl = 'http://localhost:5090/api';

  constructor(private readonly http: HttpClient) { }

  getComments(key: string): Observable<CommentDto[]> {
    return this.http.get<CommentDto[]>(`${this.baseUrl}/games/${key}/comments`);
  }
}

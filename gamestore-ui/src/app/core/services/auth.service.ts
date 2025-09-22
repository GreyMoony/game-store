import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5090/api';

  constructor(private readonly http: HttpClient) { }

  login(): Observable<{ token: string }> {
    // Hardcoded credentials until we have a proper login form
    const credentials = {
      model:
      {
        login: 'admin',
        password: 'Admin@123',
        internalAuth: true
      }
    };

    return this.http.post<{ token: string }>(`${this.baseUrl}/users/login`, credentials);
  }
}

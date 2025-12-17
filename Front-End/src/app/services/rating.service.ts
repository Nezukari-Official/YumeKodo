import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  private ApiUrl = 'https://localhost:7242/api/Rating';

  constructor(private http: HttpClient, private authService: AuthService) { }

  AddRating(accountId: number, gameId: number, starRating: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    const url = `${this.ApiUrl}/Add-Rating?accountId=${accountId}&gameId=${gameId}&starRating=${starRating}`;
    return this.http.post<any>(url, {}, { headers });
  }

  GetGameRatings(gameId: number): Observable<any> {
    return this.http.get<any>(`${this.ApiUrl}/Get-Game-Ratings/${gameId}`);
  }

  GetUserRating(accountId: number, gameId: number): Observable<any> {
    return this.http.get<any>(`${this.ApiUrl}/Get-User-Rating?accountId=${accountId}&gameId=${gameId}`);
  }
}

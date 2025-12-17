import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private ApiUrl = 'https://localhost:7242/api/Game';

  constructor(private http: HttpClient, private authService: AuthService) { }

  GetAllGames(genre?: string, minRating?: number): Observable<any> {
    let url = `${this.ApiUrl}/Get-All-Games`;
    const params: string[] = [];

    if (genre) params.push(`genre=${encodeURIComponent(genre)}`);
    if (minRating) params.push(`minRating=${minRating}`);

    if (params.length > 0) {
      url += `?${params.join('&')}`;
    }

    return this.http.get<any>(url);
  }

  GetGenres(): Observable<any> {
    return this.http.get<any>(`${this.ApiUrl}/Get-Genres`);
  }

  GetGameById(id: number): Observable<any> {
    return this.http.get<any>(`${this.ApiUrl}/Get-Game-By-Id?Id=${id}`);
  }

  DownloadGame(gameId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.ApiUrl}/Download-Game/${gameId}`, { headers });
  }

  AddNewGame(gameData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();

    const formattedData = {
      Title: gameData.title,
      Description: gameData.description,
      Genre: gameData.genre,
      ThumbnailURL: gameData.thumbnailURL,
      DownloadURL: gameData.downloadURL
    };

    return this.http.post<any>(`${this.ApiUrl}/Add-New-Game`, formattedData, { headers });
  }

  DeleteGame(gameId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.delete<any>(`${this.ApiUrl}/Delete-Game/${gameId}`, { headers });
  }
}

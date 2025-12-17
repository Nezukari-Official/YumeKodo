import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private ApiUrl = 'https://localhost:7242/api/Comment';

  constructor(private http: HttpClient, private authService: AuthService) { }

  AddComment(accountId: number, gameId: number, commentText: string): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    const url = `${this.ApiUrl}/Add-Comment?accountId=${accountId}&gameId=${gameId}&commentText=${encodeURIComponent(commentText)}`;
    return this.http.post<any>(url, {}, { headers });
  }

  GetGameComments(gameId: number): Observable<any> {
    return this.http.get<any>(`${this.ApiUrl}/Get-Game-Comments/${gameId}`);
  }

  DeleteComment(commentId: number, requestingUserId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    const url = `${this.ApiUrl}/Delete-Comment/${commentId}?requestingUserId=${requestingUserId}`;
    return this.http.delete<any>(url, { headers });
  }
}

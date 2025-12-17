import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private ApiUrl = 'https://localhost:7242/api/Account';


  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient) {
    this.checkExistingToken();
  }

  SignUp(userData: any): Observable<any> {
    return this.http.post<any>(`${this.ApiUrl}/Sign-Up`, userData);
  }

  SignIn(data: { Email: string, PasswordHash: string }): Observable<any> {
    return this.http.post<any>(`${this.ApiUrl}/Sign-In`, {
      Email: data.Email,
      PasswordHash: data.PasswordHash
    }).pipe(
      tap((response) => {
        if (response.status === 200 && response.data?.token) {
          this.setToken(response.data.token);
          this.setUserLoggedIn(true);

          const userInfo = this.decodeToken(response.data.token);
          this.currentUserSubject.next(userInfo);

          console.log('User signed in successfully', userInfo);
        }
      })
    );
  }

  SignOut(): void {
    this.removeToken();
    this.setUserLoggedIn(false);
    this.currentUserSubject.next(null);
    console.log('User signed out');
  }

  setToken(token: string): void {
    const cleanToken = token.replace(/^['"]|['"]$/g, '');
    localStorage.setItem('authToken', cleanToken);
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  removeToken(): void {
    localStorage.removeItem('authToken');
  }

  private checkExistingToken(): void {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      this.setUserLoggedIn(true);
      const userInfo = this.decodeToken(token);
      this.currentUserSubject.next(userInfo);
    } else if (token && this.isTokenExpired(token)) {
      this.removeToken();
    }
  }

  
  private decodeToken(token: string): any {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        accountId: payload.AccountId || payload.nameid,
        nickname: payload.name,
        email: payload.email,
        role: payload.Role,
        profileImageURL: payload.ProfileImageURL ? decodeURIComponent(payload.ProfileImageURL) : null,
        exp: payload.exp
      };
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }


  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp < currentTime;
    } catch (error) {
      return true;
    }
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    return token != null && !this.isTokenExpired(token);
  }

  private setUserLoggedIn(status: boolean): void {
    this.isLoggedInSubject.next(status);
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  canAddGames(): boolean {
    const user = this.getCurrentUser();
    return user?.role === 'Creator' || user?.role === 'Admin';
  }

  VerifyEmail(email: string, verificationCode: string): Observable<any> {
    return this.http.post<any>(`${this.ApiUrl}/Verify-Email/${email}/${verificationCode}`, {});
  }

  GetPasswordResetCode(email: string): Observable<any> {
    return this.http.post<any>(`${this.ApiUrl}/Get-Password-Reset-Code`, { AccountEmail: email });
  }

  ResetPassword(email: string, code: string, newPassword: string): Observable<any> {
    return this.http.put<any>(`${this.ApiUrl}/Reset-Password`, {
      Email: email,
      Code: code,
      NewPassword: newPassword
    });
  }

  GetAccountById(id: number): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.get<any>(`${this.ApiUrl}/Get-Account-By-Id?Id=${id}`, { headers });
  }

  UpdateProfile(email: string, newNickname: string, newEmail: string, newProfileImage: string): Observable<any> {
    const headers = this.getAuthHeaders();

    const profileImage = newProfileImage && newProfileImage.trim() ? newProfileImage : 'null';

    const url = `${this.ApiUrl}/Update-Profile?Email=${encodeURIComponent(email)}&NewNickname=${encodeURIComponent(newNickname)}&NewEmail=${encodeURIComponent(newEmail)}&NewProfileImage=${encodeURIComponent(profileImage)}`;

    return this.http.put<any>(url, {}, { headers });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { UserDto } from '../models/user-dto';
import { TokenResponseDto } from '../models/token-response-dto';
import { RefreshTokenRequestDto } from '../models/refresh-token-request-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7234/api/auth';

  private refreshTokenRequestDto: RefreshTokenRequestDto = {
    userId: '',
    refreshToken: ''
  };

  login(userDto: UserDto): Observable<TokenResponseDto> {
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/login`, userDto)
      .pipe(
        tap(response => {
          localStorage.setItem('auth_token', response.accessToken);
          localStorage.setItem('refresh_token', response.refreshToken);
        })
      );
  }

  refreshToken(refreshToken: string | null, userId: string | null): Observable<TokenResponseDto> {
    refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token found');
    }

    userId = this.getCurrentUserId();
    if (!userId) {
      throw new Error('No user Id found');
    }

    this.refreshTokenRequestDto.refreshToken = refreshToken;
    this.refreshTokenRequestDto.userId = userId;

    return this.http.post<TokenResponseDto>(`${this.apiUrl}/refresh-token`, this.refreshTokenRequestDto).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
      })
    );
  }

  private setTokens(accessToken: string, refreshToken?: string): void {
    localStorage.setItem('auth_token', accessToken);
    if (refreshToken) {
      localStorage.setItem('refresh_token', refreshToken);
    }
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    // Remove tokens from local storage immediately
    localStorage.removeItem('auth_token');
    localStorage.removeItem('refresh_token');
    // TODO: notify server to revoke refresh token
    // return this.http.post(`${this.apiUrl}/logout`, { refreshToken });
  }

  getCurrentUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = this.parseJwt(token);
    if (!payload) return null;

    return (
      payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
      null
    );
  }

  private parseJwt(token: string): any {
    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = this.base64UrlDecode(payloadBase64);
      return JSON.parse(payloadJson);
    } catch {
      return null;
    }
  }

  private base64UrlDecode(str: string): string {
    str = str.replace(/-/g, '+').replace(/_/g, '/');
    while (str.length % 4) {
      str += '=';
    }
    return atob(str);
  }
}

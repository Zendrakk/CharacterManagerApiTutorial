import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { UserDto } from '../models/user-dto';
import { TokenResponseDto } from '../models/token-response-dto';
import { RefreshTokenRequestDto } from '../models/refresh-token-request-dto';
import { LogoutRequest } from '../models/logout-request';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // HttpClient is injected for making HTTP requests to the backend API
  private http = inject(HttpClient);

  // Base URL for authentication-related API endpoints
  private apiUrl = 'https://localhost:7234/api/auth';

  // Data Transfer Object used when requesting a new access token with an existing refresh token
  private refreshTokenRequestDto: RefreshTokenRequestDto = {
    userId: '',
    refreshToken: ''
  };

  // DTO used when logging out a user, containing the refresh token to invalidate
  private logoutRequest: LogoutRequest = {
    refreshToken: ''
  };


  /**
   * Sends a login request to the API and stores authentication tokens.
   * Returns the response as an observable for further processing or subscription.
   */
  login(userDto: UserDto): Observable<TokenResponseDto> {
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/login`, userDto)
      .pipe(
        tap(response => {
          localStorage.setItem('auth_token', response.accessToken);
          localStorage.setItem('refresh_token', response.refreshToken);
        })
      );
  }


  /**
   * Attempts to refresh the user's authentication tokens using the stored refresh token.
   */
  refreshToken(refreshToken: string | null, userId: string | null): Observable<TokenResponseDto> {
    // Always retrieve the refresh token from storage/service instead of using the passed argument
    refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token found'); // Ensure a token exists
    }

    // Always retrieve the current user ID from storage/service instead of using the passed argument
    userId = this.getCurrentUserId();
    if (!userId) {
      throw new Error('No user Id found'); // Ensure a user ID exists
    }

    // Update the DTO with the refresh token and userId for the API request
    this.refreshTokenRequestDto.refreshToken = refreshToken;
    this.refreshTokenRequestDto.userId = userId;

    // Send POST request to backend refresh-token endpoint
    return this.http.post<TokenResponseDto>(`${this.apiUrl}/refresh-token`, this.refreshTokenRequestDto).pipe(
      // On successful response, update stored access and refresh tokens
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
      })
    );
  }

 /**
  * Stores access and refresh tokens in local storage.
  * 
  * @param accessToken - JWT access token required for API requests.
  * @param refreshToken - Optional refresh token used to obtain a new access token.
  */
  private setTokens(accessToken: string, refreshToken?: string): void {
    localStorage.setItem('auth_token', accessToken);
    if (refreshToken) {
      localStorage.setItem('refresh_token', refreshToken); // Save refresh token only if provided
    }
  }

  /**
   * Retrieves the current access token from local storage.
   * @returns The stored access token, or null if none exists.
   */
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  /**
   * Retrieves the current refresh token from local storage.
   * @returns The stored refresh token, or null if none exists.
   */
  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  /**
   * Checks whether the user is currently logged in.
   * @returns True if an access token exists in local storage, false otherwise.
   */
  isLoggedIn(): boolean {
    return !!this.getToken();
  }


  /**
   * Logs out user by revoking their refresh token on the server, then clearing authentication tokens from local storage.
   */
  logout(): Observable<boolean> {
    this.logoutRequest.refreshToken = this.getRefreshToken();

    // If there is no refresh token, we can just return false immediately
    if (!this.logoutRequest.refreshToken) {
      return new Observable((observer) => {
        observer.next(false);
        observer.complete();
      });
    }
    // Send refresh token to server and then clear from local storage
    return this.http.post<boolean>(`${this.apiUrl}/logout`, this.logoutRequest).pipe(
      tap((success) => {
        if (success) {
          localStorage.removeItem('auth_token');
          localStorage.removeItem('refresh_token');
        } else {
          console.error('Server failed to revoke refresh token.');
        }
      })
    );
  }


  /**
   * Extracts the current user's ID from the stored JWT token.
   * @returns The user ID if available, otherwise null.
   */
  getCurrentUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      // Decode the JWT payload
      const payloadBase64 = token.split('.')[1];
      if (!payloadBase64) return null;

      // Replace URL-safe characters and pad
      const base64 = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
      const paddedBase64 = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=');

      // Decode using browser's atob
      const decoded = atob(paddedBase64);
      const payload = JSON.parse(decoded);

      // Return the "nameidentifier" claim or null if not present
      return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
    } catch {
        return null;
    }
  }
}

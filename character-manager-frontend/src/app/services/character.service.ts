import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { CharacterDTO } from '../models/character-dto';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  public characterDtosSignal = signal<CharacterDTO[]>([]);
  public loadingSignal = signal<boolean>(false);
  public errorSignal = signal<string | null>(null);

  private apiUrl: string = 'https://localhost:7234/api/character';

  constructor(private http: HttpClient) {}


  /** Get all characters with enriched lookup names directly from server */
  getCharacters(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.http.get<CharacterDTO[]>(this.apiUrl).pipe(
      catchError(err => {
        if (err.error) {
          this.errorSignal.set('A server error occurred.');
          console.log('A server error occurred.');
        } else {
          this.errorSignal.set('An unknown error occurred.');
          console.log('An unknown error occurred.');
        }
        return of([]);  // return empty array so the observable still completes
      }),
      finalize(() => this.loadingSignal.set(false))
    ).subscribe(data => {
      this.characterDtosSignal.set(data); // store in signal
    });
  }

  /** Delete a character by ID */
  deleteCharacter(id: number): Observable<boolean> {
    this.errorSignal.set(null);
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`).pipe(
      tap(success => {
        if (success) {
          console.log("YOLO: " + success)
          // Optimistically update client state by removing deleted character
          const updated = this.characterDtosSignal().filter(c => c.id !== id);
          this.characterDtosSignal.set(updated);
        }
      }),
      catchError(err => {
        this.errorSignal.set('Failed to delete character.');
        return throwError(() => err);
      })
    );
  }
}

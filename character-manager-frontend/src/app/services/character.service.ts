import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { CharacterDTO } from '../models/character-dto';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  public characterDtos = signal<CharacterDTO[]>([]);
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  private apiUrl: string = 'https://localhost:7234/api/character';

  constructor(private http: HttpClient) {}


  /** Get all characters with enriched lookup names directly from server */
  getCharacters(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get<CharacterDTO[]>(this.apiUrl).pipe(
      catchError(err => {
        if (err.error) {
          this.error.set('A server error occurred.');
          console.log('A server error occurred.');
        } else {
          this.error.set('An unknown error occurred.');
          console.log('An unknown error occurred.');
        }
        return of([]);  // return empty array so the observable still completes
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(data => {
      this.characterDtos.set(data); // store in signal
    });
  }
}

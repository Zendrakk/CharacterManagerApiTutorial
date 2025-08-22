import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { CharacterDTO } from '../models/character-dto';
import { CharacterDisplayDTO } from '../models/character-display-dto'
import { LookupDataService } from './lookup-data.service';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  public characterDisplayDtosSignal = signal<CharacterDisplayDTO[]>([]);
  public characterDtosSignal = signal<CharacterDTO[]>([]);
  public loadingSignal = signal<boolean>(false);
  public errorSignal = signal<string | null>(null);

  private apiUrl: string = 'https://localhost:7234/api/character';

  constructor(
    private http: HttpClient,
    private lookupDataService: LookupDataService
  ) {}


  /** Get all characters with enriched lookup names directly from server */
  public getCharacters(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.http.get<CharacterDisplayDTO[]>(this.apiUrl).pipe(
      catchError((err: unknown) => {
        let errorMsg = 'An unknown error occurred.';

        if (err instanceof HttpErrorResponse) {
          if (err.status === 0) {
            // Network error (request didn’t reach the server)
            errorMsg = 'A network error occurred. Please check your connection.';
            console.error('Network error:', err);
          } else {
            // Backend returned an error response
            errorMsg = `Server error ${err.status}: ${err.message}`;
            console.error('Backend error:', err);
          }
        } else {
          // Unexpected error type (should rarely happen)
          console.error('Unexpected error type:', err);
        }
        
        this.errorSignal.set(errorMsg);
        return of([]);  // return empty array so the observable still completes
      }),
      finalize(() => this.loadingSignal.set(false))
    ).subscribe(data => {
      this.characterDisplayDtosSignal.set(data); // store in signal
    });
  }

  /** Update a character */
  public updateCharacter(updated: CharacterDTO): Observable<CharacterDTO> {
    this.errorSignal.set(null);
    return this.http.put<CharacterDTO>(`${this.apiUrl}/${updated.id}`, updated).pipe(
      tap(saved => {
        // Update the signal
        this.characterDtosSignal.update(chars =>
          chars.map(c => (c.id === saved.id ? saved : c))
        );
      }),
      catchError(err => {
        console.error(`Failed to update character ${updated.id}:`, err);
        this.errorSignal.set(`Failed to update character ${updated.id}:`);
        return throwError(() => err);
      })
    );
  }

  /**
   * Updates (or inserts) a Character in the characterDisplayDtosSignal state.
   * - First maps the raw CharacterDTO into a display-ready DTO (with names).
   * - Then updates the signal array immutably so Angular change detection works.
   */
  public updateCharacterInSignal(char: CharacterDTO): void {
    // Convert the raw CharacterDTO into a CharacterDisplayDTO with names
    const charDisplay = this.mapToDisplayDTO(char);

    // Update the signal state (characterDisplayDtosSignal)
    this.characterDisplayDtosSignal.update(chars => {
      // Find index of existing character with the same ID
      const index = chars.findIndex(c => c.id === char.id);
      if (index > -1) {
        // Character already exists → replace it in-place
        chars[index] = charDisplay;
      } else {
        // Character does not exist → add it to the array
        chars.push(charDisplay);
      }

      // Return a new array reference to trigger Angular change detection
      return [...chars];
    });
  }

  /** Delete a character by ID */
  public deleteCharacter(id: number): Observable<boolean> {
    this.errorSignal.set(null);
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`).pipe(
      tap(success => {
        if (success) {
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

  /**
   * Map CharacterDTO → CharacterDisplayDTO using the lookup signal
   */
  private mapToDisplayDTO(char: CharacterDTO): CharacterDisplayDTO {
    // Get the current lookup data from the service (realms, races, classes, factions).
    const lookup = this.lookupDataService.lookupDataDtoSignal();

    if (!lookup) {
      console.warn('Lookup data not ready yet when mapping character:', char);
      return { ...char, realmName: '', raceName: '', className: '', factionName: '' };
    }

    // Build a new object based on the incoming character, spreading the original properties and adding new display fields.
    const mappedCharacterDisplayDTO = {
      ...char,
      realmName:   lookup?.realms.find(r => r.id === Number(char.realmId))?.name ?? '',
      raceName:    lookup?.raceTypes.find(r => r.id === Number(char.raceId))?.name ?? '',
      className:   lookup?.classTypes.find(c => c.id === Number(char.classId))?.name ?? '',
      factionName: lookup?.factionTypes.find(f => f.id === Number(char.factionId))?.name ?? ''
    };

    // Return the enriched display-ready DTO
    return mappedCharacterDisplayDTO;
  }
}

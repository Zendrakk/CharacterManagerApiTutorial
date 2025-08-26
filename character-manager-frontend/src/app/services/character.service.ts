import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { CharacterDTO } from '../models/character-dto';
import { CharacterDisplayDTO } from '../models/character-display-dto'
import { LookupDataService } from './lookup-data.service';
import { DeleteResponse } from '../models/delete-response';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  /**
   * Holds the list of characters currently available in the client state. Updated whenever characters are fetched, created, updated, or deleted.
   */
  public characterDisplayDtosSignal = signal<CharacterDisplayDTO[]>([]);

  /**
   * Indicates whether a character-related request (fetch, create, update, delete) is currently in progress. 
   */
  public loadingSignal = signal<boolean>(false);

  /**
   * Stores the most recent error message, if any, from a character operation. Used by the UI to display error messages.
   */
  public errorSignal = signal<string | null>(null);

  /**
   * The base API URL for all character-related HTTP requests.
   */
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
      catchError(err => {
        this.handleError(err);
        return of([]); // return empty array so the observable still completes
      }),
      // Always reset loading
      finalize(() => this.loadingSignal.set(false))
    ).subscribe(data => {
      this.characterDisplayDtosSignal.set(data); // update signal
    });
  }

  /**
   * Updates an existing character on the server and synchronizes the local signal state.
   */
  public updateCharacter(updated: CharacterDTO): Observable<CharacterDTO | null> {
    // Clear any previous error before making a new request
    this.errorSignal.set(null);

    // Send PUT request to backend API with updated character data
    return this.http.put<CharacterDTO>(`${this.apiUrl}/${updated.id}`, updated).pipe(
      // On success, update the local characters signal so UI reflects the changes
      tap(() => {
        this.updateCharacterInSignal(updated);
      }),
      // On error, handle it centrally and recover gracefully by returning `null`
      catchError(err => {
        // Use the centralized error handler
        this.handleError(err);
        return of(null);
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

  /**
   * Deletes a character by ID from the server and updates the local state.
   */
  public deleteCharacter(id: number): Observable<DeleteResponse> {
    // Clear any existing error before starting a new request
    this.errorSignal.set(null);

    // Send DELETE request to the backend API for the given character ID
    return this.http.delete<DeleteResponse>(`${this.apiUrl}/${id}`).pipe(
      // On success, remove the character from local state
      tap(response => {
        if (response) {
          // Update client state by removing deleted character
          const updated = this.characterDisplayDtosSignal().filter(c => c.id !== id);
          this.characterDisplayDtosSignal.set(updated);
        }
      }),
      // On error, set a user-friendly error message and rethrow the error
      catchError(err => {
        // Use the centralized error handler
        this.handleError(err);
        return throwError(() => err); // allow subscriber to handle failure
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

  /**
   * Centralized error handler inspects the shape of the error response and normalizes it
   * into a single string that can be displayed in the UI via `errorSignal`.
   */
  private handleError(err: unknown): void {
    let messages: string[] = [];

    if (err instanceof HttpErrorResponse) {
      console.log("ERROR: ", err)
       if (err.error) {
        // Case 1: If backend sends structured ModelState errors
        if (err.error.errors && typeof err.error.errors === 'object') {
          for (const key of Object.keys(err.error.errors)) {
            messages.push(...err.error.errors[key]);
          }
        }
        // Case 2: If backend sends an array of error messages
        else if (Array.isArray(err.error)) {
          messages = err.error;
        }
        // Case 3: If backend sends a single string
        else if (typeof err.error === 'string') {
          messages = [err.error];
        }
        // Case 4: If backend sends a wrapped message
        else if (err.error.message) {
          messages = [err.error.message];
        }
        // Case 5: If unable to contact backend in general
        else if (err.status === 0) {
          messages = ['Error contacting server. Check your connection and try again.']
        }
      }
    } else {
      // All other error types:
      messages = ['Unexpected error'];
    }
    // Add errors to the signal to display in the UI
    this.errorSignal.set(messages.join(' | '));
  }
}

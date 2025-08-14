import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, of } from 'rxjs';
import { map, tap, catchError, finalize } from 'rxjs/operators';
import { Character } from '../models/character';
import { LookupDataService } from './lookup-data.service';
import { CharacterWithNames } from '../models/character-with-names';
import { LookupDataDto } from '../models/lookup-data-dto';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  public characterWithNames = signal<CharacterWithNames[]>([]);
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);
  private apiUrl: string = 'https://localhost:7234/api/character';
  private lookupCache: LookupDataDto | null = null; // cache for lookup data

  constructor(
    private http: HttpClient,
    private lookupDataService: LookupDataService
  ) {}

  /** Fetch lookup data and characters in parallel, then enrich characters */
  fetchCharacters(): void {

    // If lookupCache exists, use it to avoid fetching again. Otherwise, fetch from the lookup data service and store in cache.
    const lookup$ = this.lookupCache
      ? of(this.lookupCache)
      : this.lookupDataService.fetchLookupData().pipe(
          // Save the retrieved lookup data into cache
          tap(data => this.lookupCache = data),
          // Catch any errors fetching lookup data, log them, and fallback to null
          catchError(err => {
            console.error('Lookup fetch failed', err);
            return of(null);
          })
        );

    // Fetch characters from the API, catch errors and fallback to an empty array
    const characters$ = this.http.get<Character[]>(this.apiUrl).pipe(
      catchError(err => {
        console.error('Character fetch failed', err);
        return of([] as Character[]);
      })
    );

    // Set loading and clear previous errors
    this.loading.set(true);
    this.error.set(null);

    // Perform parallel requests using forkJoin
    forkJoin({ lookup: lookup$, characters: characters$ }).pipe(
      // Map: transform the combined result of forkJoin. If lookup is null, sets an error message
      map(({ lookup, characters }): CharacterWithNames[] => {
        if (!lookup) {
          this.error.set('Failed to load lookup data');
        }

        // Map each Character to CharacterWithNames
        return characters.map(c => ({
          ...c, // Copy original character properties
          className: lookup?.classTypes.find(ct => ct.id === c.classId)?.name ?? '',
          raceName: lookup?.raceTypes.find(rt => rt.id === c.raceId)?.name ?? '',
          factionName: lookup?.factionTypes.find(ft => ft.id === c.factionId)?.name ?? '',
          realmName: lookup?.realms.find(r => r.id === c.realmId)?.name ?? ''
        }));
      }),

      // Tap: perform side-effect of updating the signal with enriched characters
      tap(enriched => this.characterWithNames.set(enriched)),

      // catchError: handle any unexpected errors in the pipeline, return an empty array to keep the observable stream alive
      catchError(err => {
        console.error('Unexpected error in fetchCharacters', err);
        this.error.set('Failed to load characters');
        return of([] as CharacterWithNames[]);
      }),

      // finalize: always run cleanup logic when observable completes or errors, reset loading state to false
      finalize(() => this.loading.set(false))
    ).subscribe(); // Execute the observable
  }
}

import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LookupDataDto } from '../models/lookup-data-dto';

@Injectable({
  providedIn: 'root'
})
export class LookupDataService {
  private readonly apiUrl = 'https://localhost:7234/api/lookupdata';

  // Signal holds the cached data
  private _lookupDataDtoSignal = signal<LookupDataDto | null>(null);
  public lookupDataDtoSignal = this._lookupDataDtoSignal.asReadonly();

  constructor(private http: HttpClient) {}

  /**
   * Fetch lookup data from the server.
   * Uses cache if data has already been loaded.
   */
  fetchLookupData(): void {
    // Return cached data if available
    if (this._lookupDataDtoSignal()) return; // already cached


    // Otherwise, fetch from server and cache it in the signal
    this.http.get<LookupDataDto>(this.apiUrl).subscribe({
      next: data => this._lookupDataDtoSignal.set(data),
      error: err => console.error('Lookup fetch failed', err)
    });
  }

  /**
   * Force refresh the lookup data from the server, updating the cache.
   */
  refreshLookupData(): Observable<LookupDataDto> {
    return this.http.get<LookupDataDto>(this.apiUrl).pipe(
      tap(data => this._lookupDataDtoSignal.set(data))
    );
  }
}
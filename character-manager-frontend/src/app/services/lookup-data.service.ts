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
  private _lookupData = signal<LookupDataDto | null>(null);

  // Public getter to read the cached data
  public get lookupData(): LookupDataDto | null {
    return this._lookupData();
  }

  constructor(private http: HttpClient) {}

  /**
   * Fetch lookup data from the server.
   * Uses cache if data has already been loaded.
   */
  fetchLookupData(): Observable<LookupDataDto> {
    // Return cached data if available
    if (this._lookupData()) {
      return of(this._lookupData()!); // '!' is safe because we checked above
    }

    // Otherwise, fetch from server and cache it in the signal
    return this.http.get<LookupDataDto>(this.apiUrl).pipe(
      tap(data => this._lookupData.set(data))
    );
  }

  /**
   * Force refresh the lookup data from the server, updating the cache.
   */
  refreshLookupData(): Observable<LookupDataDto> {
    return this.http.get<LookupDataDto>(this.apiUrl).pipe(
      tap(data => this._lookupData.set(data))
    );
  }
}

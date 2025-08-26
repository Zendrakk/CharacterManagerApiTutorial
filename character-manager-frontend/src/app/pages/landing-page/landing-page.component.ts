import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../services/character.service';
import { LogoutButtonComponent } from "../../logout-button/logout-button.component";
import { CharacterDisplayDTO } from '../../models/character-display-dto';
import { CharacterEditComponent } from "../character-edit/character-edit.component";
import { finalize } from 'rxjs';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, LogoutButtonComponent, CharacterEditComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent implements OnInit {
  /**
   * Holds the currently selected character's ID.
   * 
   * - Private signal so it cannot be modified directly outside this service.
   * - Initialized to `null` meaning no character is currently selected.
   */
  private _selectedCharacterIdSignal = signal<number | null>(null);

  /**
   * Public readonly view of the selected character ID.
   * 
   * - Exposed to components so they can react to selection changes.
   * - Prevents external code from accidentally mutating the value.
   * - Components can subscribe/read this but must call service methods to update it.
   */
  public selectedCharacterIdSignal = this._selectedCharacterIdSignal.asReadonly();

  constructor(
    public characterService: CharacterService
  ) {}

  ngOnInit(): void {
    this.characterService.getCharacters();
  }

  /**
   * Handles when a character is selected from the UI.
   * 
   * - Updates the reactive signal that stores the currently selected character's ID.
   * - This allows other parts of the app (e.g., detail view, edit form) to reactively respond to the selection change.
   */
  public onSelect(char: CharacterDisplayDTO | null): void {
    this.setSelectedCharacterId(char ? char.id : null);
  }

   /**
   * Sets the currently selected character ID in the signal.
   */
  private setSelectedCharacterId(charId: number | null): void {
    this._selectedCharacterIdSignal.set(charId);
  }

  /**
   * Attempts to delete a character by ID and updates UI state accordingly.
   * 
   * - Calls the service layer (`characterService.deleteCharacter`) to make the HTTP DELETE request.
   * - Uses `finalize` to ensure that cleanup logic (like deselecting the character) runs whether the request succeeds or fails.
   * - Subscribes to the observable.
   */
  public deleteCharacter(id: number) {
    this.characterService.deleteCharacter(id).pipe(
      finalize(() => {
        // Runs no matter what (success or error)
        console.log('Delete request finished');
        this.setSelectedCharacterId(null);  // Closes edit window if it was opened during character deletion.
      })
    ).subscribe({
      next: (response) => {
        if (response) {
          console.log('Character deleted successfully');
        } else {
          console.warn(`Failed to delete character ${id}`);
        }
      },
      error: (err) => console.error(err)
    });
  }

  /**
   * Returns a CSS class name based on the character's faction.
   */
  public getFactionClass(factionName: string): string {
    switch (factionName) {
      case "Light Side": 
        return 'faction-light';
      case "Dark Side": 
        return 'faction-dark';
      default: 
        // If faction is unknown or not styled, return no class
        return '';
    }
  }

  /**
   * Retries fetching the list of characters after an error or failed load.
   *
   * - Clears any existing characters from the `characterDisplayDtosSignal` so the UI can reset (e.g., skeleton loaders show again).
   * - Calls `getCharacters()` on the service to trigger a fresh fetch from the API.
   */
  public retryGetCharacters(): void {
    this.characterService.characterDisplayDtosSignal.set([]); // clear current data
    this.characterService.getCharacters();   // re-run fetch
  }
}

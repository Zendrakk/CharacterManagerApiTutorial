import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../services/character.service';
import { LogoutButtonComponent } from "../../logout-button/logout-button.component";
import { CharacterDisplayDTO } from '../../models/character-display-dto';
import { CharacterEditComponent } from "../character-edit/character-edit.component";

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, LogoutButtonComponent, CharacterEditComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent implements OnInit {
  selectedCharacterIdSignal = signal<number | null>(null);

  constructor(public characterService: CharacterService) {}

  ngOnInit(): void {
    this.characterService.getCharacters();
  }

  public onSelect(char: CharacterDisplayDTO) {
    this.selectedCharacterIdSignal.set(char.id);
  }

  // Call delete on button click
  public deleteCharacter(id: number) {
    this.characterService.deleteCharacter(id).subscribe({
      next: (success) => {
        if (success) {
          console.log(`Character ${id} deleted successfully`);
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
}

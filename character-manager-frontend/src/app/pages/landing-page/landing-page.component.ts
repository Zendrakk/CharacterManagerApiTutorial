import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../services/character.service';
import { LogoutButtonComponent } from "../../logout-button/logout-button.component";

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, LogoutButtonComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent implements OnInit {
  constructor(public characterService: CharacterService) {}

  ngOnInit(): void {
    this.characterService.getCharacters();
  }

  // Call delete on button click
  deleteCharacter(id: number) {
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
}

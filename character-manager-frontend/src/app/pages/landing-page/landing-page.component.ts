import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CharacterService } from '../../services/character.service';
import { Character } from '../../models/character';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent implements OnInit {
  private characterService = inject(CharacterService);
  characters: Character[] = [];
  loading = true;

  ngOnInit() {
    this.characterService.getCharacters().subscribe({
      next: (data) => {
        this.characters = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}

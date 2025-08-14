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
    // Fetch characters (lookup data will be loaded automatically in service)
    this.characterService.fetchCharacters();
  }
}

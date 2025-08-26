import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CharacterService } from '../../services/character.service';
import { LookupDataService } from '../../services/lookup-data.service';
import { CharacterDTO } from '../../models/character-dto';
import { CharacterDisplayDTO } from '../../models/character-display-dto';

@Component({
  selector: 'app-character-edit',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './character-edit.component.html',
  styleUrl: './character-edit.component.css'
})
export class CharacterEditComponent implements OnChanges {
  @Input({ required: true }) characterId!: number;
  @Output() cancel = new EventEmitter<void>();
  form!: FormGroup;
  char: CharacterDisplayDTO | null = null;


  /**
   * Component constructor
   *
   * Responsibilities:
   * - Injects required Angular services:
   *    - FormBuilder (`fb`) → used to create and manage reactive forms.
   *    - CharacterService (`characterService`) → handles CRUD operations for characters.
   *    - LookupDataService (`lookupDataService`) → provides lookup data (realms, races, classes, factions).
   *
   * - Initializes the reactive form with form controls and validators.
   *   Each field represents a property of a character:
   *    - `name`: required string input, constrained to only letters, and a length between 3 and 15 characters.
   *    - `level`: required number input, constrained to only numbers, and a range between 1 and 50.
   *    - `realmId`, `raceId`, `classId`, `factionId`: required dropdown selections.
   */
  constructor(
    private fb: FormBuilder, 
    private characterService: CharacterService,
    public lookupDataService: LookupDataService
  ) {
    // Initialize the reactive form with default values and validation rules
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15), Validators.pattern(/^[A-Za-z]+$/)]],
      level: [1, [Validators.required, Validators.min(1), Validators.max(50), Validators.pattern(/^\d+$/)]],
      realmId: ['', [Validators.required]],
      raceId: ['', [Validators.required]],
      classId: ['', [Validators.required]],
      factionId: ['', [Validators.required]]
    });
  }


  ngOnInit(): void {
    // Request lookup data from the service when the component initializes
    this.lookupDataService.fetchLookupData();
  }

  /**
   * Lifecycle hook that runs whenever input-bound properties of the component change.
   * 
   * In this case, it reacts to changes in the `characterId` input:
   * 
   * - If `characterId` changes and is defined:
   *   1. Look up the corresponding character object from the signal (`characterDisplayDtosSignal`).
   *   2. If the character exists:
   *      - Capitalize the first letter of the character’s name.
   *      - Patch the form with the character’s details (name, level, realm, race, class, faction).
   *        Only existing form controls will be patched (safe operation).
   *   3. If the character does not exist:
   *      - Reset the form to clear any stale data.
   */
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['characterId'] && this.characterId){
      // Find the character that matches the current characterId
      this.char = this.characterService.characterDisplayDtosSignal()
      .find(c => c.id === this.characterId) ?? null;

      if (this.char) {
        // Capitalize the first letter of the character's name for UI
        let capName = this.char.name;
        capName = capName.charAt(0).toUpperCase() + capName.slice(1);

        // Populate form fields with the character’s data
        this.form.patchValue({
          name: capName,
          level: this.char.level,
          realmId: this.char.realmId,
          raceId: this.char.raceId,
          classId: this.char.classId,
          factionId: this.char.factionId
        });
      } else {
        // Reset form if no matching character is found
        this.form.reset();
      }
    }
  }

  /**
   * Saves the currently edited character to the backend.
   *
   * Workflow:
   * 1. First checks if the form is invalid OR if `characterId` is missing.
   *    - If either is true, exits early without making a request.
   * 2. Constructs a `CharacterDTO` object using:
   *    - The bound `characterId` (from @Input or signal).
   *    - Form field values (name, level, realmId, raceId, classId, factionId).
   * 3. Calls the `updateCharacter` method in the CharacterService to send an update request to the server.
   * 4. Subscribes to the update observable:
   *    - On success (`next`): invokes `onCancel()` to close the edit form once the update completes.
   *    - On error: logs the error to the console for debugging.
   */
  public save(): void {
    if (this.form.invalid || this.characterId == null) return;
    
    const updatedChar: CharacterDTO = {
      id: this.characterId,                 // From @Input() or signal
      name: this.form.value.name,           // From form field
      level: this.form.value.level,         // From form field
      realmId: this.form.value.realmId,     // From form field
      raceId: this.form.value.raceId,       // From form field
      classId: this.form.value.classId,     // From form field
      factionId: this.form.value.factionId  // From form field
    };

    this.characterService.updateCharacter(updatedChar).subscribe({
      next: () => {
        this.onCancel(); // Closes the Edit screen upon character update
      },
      error: err => console.error('Update failed', err)
    });
  }

  // Closes the Edit screen
  public onCancel(): void {
    this.cancel.emit();
  }
}

import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CharacterService } from '../../services/character.service';
import { LookupDataService } from '../../services/lookup-data.service';
import { CharacterDTO } from '../../models/character-dto';
import { CharacterDisplayDTO } from '../../models/character-display-dto';
import { LookupDataDto } from '../../models/lookup-data-dto';

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


  constructor(
    private fb: FormBuilder, 
    private characterService: CharacterService,
    public lookupDataService: LookupDataService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      level: [1, [Validators.required, Validators.min(1), Validators.max(50)]],
      realmId: ['', Validators.required],
      raceId: ['', Validators.required],
      classId: ['', Validators.required],
      factionId: ['', Validators.required]
    });
  }


  ngOnInit(): void {
    this.lookupDataService.fetchLookupData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['characterId'] && this.characterId){
      this.char = this.characterService.characterDisplayDtosSignal()
      .find(c => c.id === this.characterId) ?? null;

      if (this.char) {
        let capName = this.char.name;
        capName = capName.charAt(0).toUpperCase() + capName.slice(1);

        // Only patch the form fields that exist
        this.form.patchValue({
          name: capName,
          level: this.char.level,
          realmId: this.char.realmId,
          raceId: this.char.raceId,
          classId: this.char.classId,
          factionId: this.char.factionId
        });
      } else {
        // Optional: reset form if character not found
        this.form.reset();
      }
    }
  }

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
        console.log('Character updated');
        this.characterService.updateCharacterInSignal(updatedChar);
        this.onCancel();
      },
      error: err => console.error('Update failed', err)
    });
  }

  public onCancel(): void {
    this.cancel.emit();
  }
}

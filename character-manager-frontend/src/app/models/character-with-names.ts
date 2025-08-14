import { Character } from "./character";

export interface CharacterWithNames extends Character {
  raceName: string;
  factionName: string;
  className: string;
  realmName: string;
}
import { ClassType } from './class-type';
import { RaceType } from './race-type';
import { FactionType } from './faction-type';
import { Realm } from './realm';

export interface LookupDataDto {
  classTypes: ClassType[];
  raceTypes: RaceType[];
  factionTypes: FactionType[];
  realms: Realm[];
}

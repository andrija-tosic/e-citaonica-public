import { Modul } from 'src/app/models/modul.model';
import { Diskusija } from 'src/app/models/diskusija.model';
import { Zadatak } from 'src/app/models/zadatak.model';
import { Blanket } from "./blanket.model";
import { Oblast } from "./oblast.model";
import { Profesor } from "./profesor.model";

export interface Predmet {
  id: number,
  naziv: string,
  semestar: number,
  godina: number,
  opis: string,
  pracen: boolean,
  profesori?: Profesor[],
  oblasti?: Oblast[],
  blanketi?: Blanket[],
  zadaci?: Zadatak[],
  diskusije?: Diskusija[],
  moduli?: Modul[]
}
import { Diskusija } from 'src/app/models/diskusija.model';
import { Oblast } from './oblast.model';
import { Blanket } from "./blanket.model";
import { Dodatak } from "./dodatak.model";
import { Komentar } from "./komentar.model";

export interface Zadatak {
  id?: number,
  redniBr?: number,
  tekst: string,
  tip: string,
  brPoena: number,
  oblasti: Oblast[],
  resenje?: Komentar,
  blanket?: Blanket,
  dodaci?: Dodatak[],
  imaDiskusije?: boolean,
  imaDiskusijeSaResenjima?: boolean
}
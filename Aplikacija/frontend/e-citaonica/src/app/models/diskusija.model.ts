import { Objava } from './objava.model';
import { Oblast } from './oblast.model';
import { Zadatak } from './zadatak.model';

export interface Diskusija extends Objava {
  naslov: string;
  zavrsena: boolean;
  pracena: boolean;
  brKomentara: number;
  zadatak?: Zadatak,
  oblasti?: Oblast[];
  oblastiIds?: number[];
  tip: string;
  zadatakId?: number
}

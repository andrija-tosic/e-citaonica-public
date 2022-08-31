import { Obavestenje } from './obavestenje.model';
import { Predmet } from "./predmet.model";

export interface UserBasic {
  id?: number,
  ime: string,
  tip: string,
  slikaURL: string,
  brZahvalnica: number,
  brResenja?: number,
  praceniPredmeti?: Predmet[] | null,
  obavestenja?: Obavestenje[]
}


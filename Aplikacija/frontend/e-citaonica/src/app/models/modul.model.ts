import { Predmet } from "./predmet.model";

export interface Modul {
  id? : number,
  naziv : string,
  predmeti? : Predmet[]
}
import { IspitniRok } from "./ispitni-rok.model";
import { Predmet } from "./predmet.model";
import { Profesor } from "./profesor.model";
import { Zadatak } from "./zadatak.model";

export interface Blanket {
  id? : number,
  ispitniRok : IspitniRok,
  datum : string,
  tip : string,
  zadaci? : Zadatak[],
  predmet? : Predmet,
  predmetId? : number
}
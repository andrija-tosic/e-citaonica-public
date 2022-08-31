import { Predmet } from "./predmet.model";
import { User } from "./user.model";

export interface Profesor extends User {
  predmeti : Predmet[]
}
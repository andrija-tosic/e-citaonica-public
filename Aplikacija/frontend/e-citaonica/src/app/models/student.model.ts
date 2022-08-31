import { Modul } from "./modul.model";
import { User } from "./user.model";

export interface Student extends User {
  indeks : number,
  modul : Modul,
  godina : number
}
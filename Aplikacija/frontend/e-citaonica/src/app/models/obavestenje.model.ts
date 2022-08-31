import { Objava } from "./objava.model";

export interface Obavestenje {
  id: number,
  sadrzaj: string,
  datumIVreme: Date,
  diskusijaId: number,
  predmetId: number,
  objavaId: number 
}
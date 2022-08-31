import { Diskusija } from "./diskusija.model";

export interface Oblast {
  // mislim da treba id jer moze da postoji link ka oblasti, mozda nije potreban link ipak
  id?: number,
  redniBr: number,
  naziv: string,
  diskusije?: Diskusija[]
}
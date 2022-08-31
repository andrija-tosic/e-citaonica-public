import { Predmet } from 'src/app/models/predmet.model';
import { Dodatak } from "./dodatak.model";
import { Komentar } from "./komentar.model";
import { UserBasic } from "./user-basic.model";
import { User } from "./user.model";

export interface Objava {
  id?: number,
  sadrzaj: string,
  datumKreiranja: Date,
  datumIzmene?: Date,
  brZahvalnica: number,
  zahvaljena?: boolean,
  arhivirana?: boolean,
  dodaci?: Dodatak[],
  autorId?: number,
  autor?: UserBasic,
  komentari?: Komentar[],
  predmet?: Predmet
}
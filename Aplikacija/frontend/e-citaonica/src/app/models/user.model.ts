import { Diskusija } from "./diskusija.model";
import { Komentar } from "./komentar.model";
import { Objava } from "./objava.model";
import { UserBasic } from "./user-basic.model";

export interface User extends UserBasic {
  email: string,
  brKomentara: number,
  brDiskusija: number,
  objave: Array<Komentar | Diskusija>,
  diskusije?: Diskusija[]
}
import { Diskusija } from "./diskusija.model";
import { Objava } from "./objava.model";
import { UserBasic } from "./user-basic.model";

export interface Komentar extends Objava {
  predlogResenja : boolean,
  prihvacen? : boolean,
  potvrdilacResenja? : UserBasic,
  objavaId? : number,
  objava?: Objava,
  diskusija?: Diskusija,
  komentari? : Komentar[]
}
import { Dodatak } from "../dodatak.model"

export interface DiskusijaAdd {
  id? : number,
  naslov : string,
  sadrzaj : string,
  tip : string,
  oblastiIds? : number[],
  zadatakId? : number,
  dodaci? : Dodatak[],
  osobeIds?: number[]
}



import { Dodatak } from "../dodatak.model";
import { Oblast } from "../oblast.model";

export interface DiskusijaEditRes {
  datumIzmene : Date,
  oblasti : Oblast[],
  dodaci: Dodatak[]
}
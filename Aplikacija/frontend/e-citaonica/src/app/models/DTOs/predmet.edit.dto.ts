import { Oblast } from "../oblast.model";

export interface PredmetEdit {
    id: number,
    naziv: string,
    opis: string,
    semestar: number,
    oblasti: Oblast[],
    profesoriIds: number[]
}
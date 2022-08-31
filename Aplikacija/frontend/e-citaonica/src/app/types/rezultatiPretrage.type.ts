import { Predmet } from '../models/predmet.model';
import { User } from '../models/user.model';


export type RezultatiPretrage = {
    predmeti: Predmet[];
    korisnici: User[];
};

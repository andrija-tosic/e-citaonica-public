import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'prettyDate' })
export class PrettyDate implements PipeTransform {
    transform(time: any): string {
        var date;

        if (time instanceof Date) {
            date = time;
        } else {
            // var temp = (time || "").replace(/-/g, "/").replace(/[T]/g, " ").split(" ");
            // var time = temp[1] ? temp[1] : '';
            // var dateTemp = temp[0];
            // dateTemp = dateTemp.split('/');
            // dateTemp = dateTemp[0] + '/' + dateTemp[1] + '/' + dateTemp[2] + ' ' + time;
            date = new Date(time);
        }

        try {
            var diff = (((new Date()).getTime() - date.getTime()) / 1000);
            var day_diff = Math.floor(diff / 86400);
        } catch (e) {
            return "Invalid Date";
        }

        if (isNaN(day_diff) || day_diff < 0)
            return "Invalid Date";

        return day_diff == 0 ?
            diff < 60 ? "Upravo sad" :
                diff < 120 ? "Pre 1 minut" :
                    diff < 3600 ? `Pre ${Math.floor(diff / 60)} minuta` :
                        diff < 7200 ? "Pre 1 sat" : 
                            diff < 14400 ? `Pre ${Math.floor(diff / 3600)} sata` : 
                                `Pre ${Math.floor(diff / 3600)} sati` :
            day_diff == 1 ? "Juče" :
                day_diff < 7 ? `Pre ${day_diff} dana` :
                    day_diff < 14 ? "Prošle nedelje" :
                        day_diff < 31 ? `Pre ${Math.ceil(day_diff / 7)} nedelje` :
                            day_diff < 365 ? `Pre ${Math.ceil(day_diff / 30)} meseca` :
                                time;
    }
}
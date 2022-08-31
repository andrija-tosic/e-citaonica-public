import { FormControl } from "@angular/forms";

export function noWhitespaceValidator(control: FormControl) {
    const rteEmpty = control.value?.replace(/<(.|\n)*?>/g, '').trim().length === 0 && !control.value?.includes("<img")
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace && !rteEmpty;
    return isValid ? null : { 'whitespace': true };
}
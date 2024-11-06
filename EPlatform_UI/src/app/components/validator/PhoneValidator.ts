import { Directive } from "@angular/core";
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator, ValidatorFn } from "@angular/forms";


@Directive({
    standalone: true,
    selector: "[phone]",
    providers: [{
        provide: NG_VALIDATORS,
        useExisting: PhoneDirective,
        multi: true
    }]
})
export class PhoneDirective implements Validator{
    validate(control: AbstractControl): ValidationErrors | null {
        return checkPhone()(control);
    }
}

export function checkPhone(): ValidatorFn {
    return (control:AbstractControl) : ValidationErrors | null => {

        const value = control.value;

        if (!value) {
            return null;
        }

        const isPhone = /^\d{8,11}$/.test(value);

        return !isPhone ? {phone:true}: null;
    }
}
using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class RegistracijaValidator : AbstractValidator<UserRegister>
    {
        public RegistracijaValidator()
        {

            RuleFor(x => x.Ime)
                .NotEmpty().WithMessage("Morate da unesete ime")
                .MinimumLength(3).WithMessage("Ime mora biti duže od 2 karaktera.")
                .Must(IsValidName).WithMessage("{PropertyName} mora biti sačinjeno samo od slova");

            RuleFor(x => x.Prezime)
                .NotEmpty().WithMessage("Morate da unesete prezime")
                .MinimumLength(3).WithMessage("Prezime mora biti duže od 2 karaktera.")
                .Must(IsValidName).WithMessage("{PropertyName} mora biti sačinjeno samo od slova");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Morate da unesete email.")
                .EmailAddress().WithMessage("Unesite validnu email adresu.");

            RuleFor(x => x.Lozinka)
                .NotEmpty().WithMessage("Morate da unesete lozinku.")
                .MinimumLength(8).WithMessage("Lozinka mora da bude najmanje 8 karaktera.");

            RuleFor(x => x.Indeks)
                .NotEmpty().WithMessage("Morate da unesete indeks.")
                .GreaterThan(0).WithMessage("Broj indeksa mora biti veći od 0.")
                .When(x => !x.JeProfesor);

            RuleFor(x => x.Godina)
               .NotEmpty().WithMessage("Morate da unesete godinu.")
               .GreaterThan(0).WithMessage("Morate da unesete godinu.")
               .When(x => !x.JeProfesor);

            RuleFor(x => x.ModulId)
               .NotEmpty().WithMessage("Morate da izaberete modul.")
               .GreaterThan(0).WithMessage("Morate da izaberete modul.")
               .When(x => !x.JeProfesor);
        }
        private bool IsValidName(string name)
        {
            return name.All(Char.IsLetter);
        }
    }
}

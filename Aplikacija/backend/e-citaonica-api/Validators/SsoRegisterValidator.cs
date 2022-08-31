using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class SsoRegisterValidator :AbstractValidator<SsoRegister>
    {
        public SsoRegisterValidator() {

            RuleFor(x => x.Indeks)
               .NotEmpty().WithMessage("Morate da unesete indeks")
               .GreaterThan(1000).WithMessage("Broj indeksa mora biti veći od 0")
               .When(x => !x.JeProfesor);

            RuleFor(x => x.Godina)
               .NotEmpty().WithMessage("Morate da unesete godinu")
               .When(x => !x.JeProfesor);

            RuleFor(x => x.ModulId)
               .NotEmpty().WithMessage("Morate da izaberete modul")
               .When(x => !x.JeProfesor);

        }
    }
}

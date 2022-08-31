using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class PredmetaValidator : AbstractValidator<PredmetModel>
    {
        public PredmetaValidator() {

            RuleFor(x => x.Naziv)
                  .NotEmpty().WithMessage("Unesite naziv predmeta");
                 

            RuleFor(x => x.Opis)
                .NotEmpty().WithMessage("Unesite opis predmeta");
               

            RuleFor(x => x.ModuliIds)
                .NotNull().WithMessage("Predmet mora biti raspoređen makar u jedan modul");

            RuleFor(x => x.ProfesoriIds)
                .NotNull().WithMessage("Na predmetu mora biti najmanje jedan profesor");

            RuleFor(x => x.Oblasti)
                .NotNull().WithMessage("Predmet mora biti podeljen u oblasti");


            RuleForEach(x => x.Oblasti).ChildRules(oblast =>
            {
                oblast.RuleFor(x => x.Naziv).NotEmpty().WithMessage("Morate uneti naziv oblasti");
            });
        }
    }
}

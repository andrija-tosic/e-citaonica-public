using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class BlanketValidator :AbstractValidator<BlanketModel>
    {
        public BlanketValidator() {

            RuleFor(x => x.Tip)
                .NotEmpty().WithMessage("Unesite tip blanketa");

            RuleFor(x => x.IspitniRokId)
                .NotEmpty().WithMessage("Unesite ispitni rok u kome je bio blanket");
            

            RuleFor(x => x.Zadaci)
               .NotNull().WithMessage("Blanket mora imati zadatke");

            RuleForEach(x => x.Zadaci).ChildRules(zadaci =>
            {
                zadaci.RuleFor(x => x.OblastiIds).NotNull().WithMessage("Morate odabrati oblast zadatka");
                zadaci.RuleFor(x => x.Tekst).NotEmpty().WithMessage("Morate da unesete tekst zadatka");
                                             
            });


        }
    }
}

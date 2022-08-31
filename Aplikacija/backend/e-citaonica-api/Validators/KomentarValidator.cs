using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class KomentarValidator : AbstractValidator<KomentarModel>
    {

        public KomentarValidator() {

            RuleFor(x => x.Sadrzaj)
                .NotEmpty().WithMessage("Morate uneti sadržaj komentara");
                
           
        
        }
    }
}

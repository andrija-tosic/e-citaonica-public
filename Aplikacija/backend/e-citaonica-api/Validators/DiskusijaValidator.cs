using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class DiskusijaValidator : AbstractValidator<DiskusijaModel>
    {
        public DiskusijaValidator()
        {
            RuleFor(x => x.Naslov)
                .NotEmpty().WithMessage("Unesite naslov diskusije");

            RuleFor(x => x.Sadrzaj)
                .NotEmpty().WithMessage("Morate uneti sadržaj diskusije");
        }
    }
}

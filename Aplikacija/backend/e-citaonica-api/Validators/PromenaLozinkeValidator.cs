using e_citaonica_api.Models;
using FluentValidation;

namespace e_citaonica_api.Validators;

public class PromenaLozinkeValidator : AbstractValidator<PromenaLozinke>
{
    public PromenaLozinkeValidator()
    {
        RuleFor(x => x.oldPassword)
            .NotEmpty().WithMessage("Lozinka ne može biti prazna")
            .MinimumLength(8).WithMessage("Lozinka mora da bude najmanje 8 karaktera");

        RuleFor(x => x.newPassword)
            .NotEmpty().WithMessage("Lozinka ne može biti prazna")
            .MinimumLength(8).WithMessage("Lozinka mora da bude najmanje 8 karaktera");

        RuleFor(x => x.newPasswordAgain)
            .NotEmpty().WithMessage("Lozinka ne može biti prazna")
            .MinimumLength(8).WithMessage("Lozinka mora da bude najmanje 8 karaktera")
            .Equal(x => x.newPassword).WithMessage("Nove lozinke se ne poklapaju");
    }
}
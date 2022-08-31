using e_citaonica_api.TransferModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.Validators
{
    public class LoginValidator :AbstractValidator<UserLogin>
    {
        public LoginValidator() {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Morate da unesete email")
                .EmailAddress().WithMessage("Unesite email adresu");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Morate da unesete lozinku")
                .MinimumLength(8).WithMessage("Lozinka mora da bude najmanje 8 karaktera"); ;
                
          
        
        
        }
    }
}

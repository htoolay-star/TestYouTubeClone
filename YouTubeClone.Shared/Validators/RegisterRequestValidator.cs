using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().UsernameRules();
            RuleFor(x => x.Email).NotEmpty().EmailRules();
            RuleFor(x => x.Password).NotEmpty().PasswordRules();
            RuleFor(x => x.ConfirmPassword).ConfirmPasswordRules(x => x.Password);
            RuleFor(x => x.DateOfBirth).NotEmpty().AgeRules();
        }
    }
}

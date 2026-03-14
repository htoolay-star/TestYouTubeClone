using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            // Use custom extension for complexity check
            RuleFor(x => x.NewPassword)
                .PasswordRules();

            // Use custom extension for comparison check
            RuleFor(x => x.ConfirmPassword)
                .ConfirmPasswordRules(x => x.NewPassword);
        }
    }
}

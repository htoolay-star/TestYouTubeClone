using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
    {
        public ChangeEmailRequestValidator()
        {
            // Use the EmailRules extension we created earlier
            RuleFor(x => x.NewEmail)
                .EmailRules();

            // Use the new ConfirmEmailRules extension
            RuleFor(x => x.ConfirmEmail)
                .ConfirmEmailRules(x => x.NewEmail);

            // Standard check for password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required to confirm identity");
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class ConfirmEmailChangeRequestValidator : AbstractValidator<ConfirmEmailChangeRequest>
    {
        public ConfirmEmailChangeRequestValidator()
        {
            RuleFor(x => x.Otp).OtpRules();
        }
    }
}

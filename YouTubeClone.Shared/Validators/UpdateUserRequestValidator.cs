using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            // PATCH ဖြစ်လို့ null မဟုတ်မှ စစ်မယ်၊ logic တွေက အပေါ်က extension ကနေလာမယ်
            RuleFor(x => x.Username).UsernameRules().When(x => x.Username != null);
            RuleFor(x => x.DateOfBirth).AgeRules().When(x => x.DateOfBirth != null);

            RuleFor(x => x.Bio).MaximumLength(500).WithMessage("Bio cannot exceed 500 characters");
            RuleFor(x => x.Gender).InclusiveBetween((byte)1, (byte)3).When(x => x.Gender != null);
        }
    }
}

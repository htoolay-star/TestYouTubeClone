using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Shared.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            // Email ကတော့ format မှန်မှ ခွင့်ပြုမယ် (Extension ကို သုံးမယ်)
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailRules();

            // Password ကိုတော့ ရိုက်ထည့်ထားဖို့ပဲ လိုတယ် (Rules တွေ အကုန်မစစ်တော့ဘူး)
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}

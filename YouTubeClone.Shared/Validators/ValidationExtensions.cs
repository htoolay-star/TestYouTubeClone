using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.Validators
{
    public static class ValidationExtensions
    {
        // Username Rules
        public static IRuleBuilderOptions<T, string?> UsernameRules<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

        // Email Rules
        public static IRuleBuilderOptions<T, string?> EmailRules<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.EmailAddress().WithMessage("Invalid email format");

        // Password Rules
        public static IRuleBuilderOptions<T, string?> PasswordRules<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                       .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                       .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

        // Age/DOB Rules
        public static IRuleBuilderOptions<T, DateOnly?> AgeRules<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder)
        {
            return ruleBuilder.Must(date =>
            {
                // Update မှာ null ပို့လာရင် Valid လို့ သတ်မှတ်ပေးရမယ် (ဘာမှမပြင်ချင်တာ ဖြစ်လို့)
                if (date == null) return true;

                var today = DateOnly.FromDateTime(DateTime.Now);
                return date <= today.AddYears(-13);
            })
            .WithMessage("You must be at least 13 years old");
        }

        // Confirm Password Rules (Needs to compare with the actual Password)
        public static IRuleBuilderOptions<T, string?> ConfirmPasswordRules<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            System.Linq.Expressions.Expression<Func<T, string?>> comparisonProperty)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Confirm Password is required")
                .Equal(comparisonProperty).WithMessage("Passwords do not match");
        }

        public static IRuleBuilderOptions<T, string?> ConfirmEmailRules<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            System.Linq.Expressions.Expression<Func<T, string?>> comparisonProperty)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Confirm email is required")
                .Equal(comparisonProperty).WithMessage("Emails do not match");
        }

        public static IRuleBuilderOptions<T, string?> OtpRules<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.NotEmpty().WithMessage("OTP is required")
               .Length(6).WithMessage("OTP must be exactly 6 digits")
               .Matches(@"^[0-9]*$").WithMessage("OTP must contain only numbers");
    }
}

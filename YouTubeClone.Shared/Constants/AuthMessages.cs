using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.Constants
{
    public static class AuthMessages
    {
        // 1. Authentication & Account
        public static class Auth
        {
            public const string UserNotFound = "User not found.";
            public const string AccountDeactivated = "Your account is deactivated. Please contact support to restore it.";
            public const string InvalidCredentials = "The email or password you entered is incorrect.";
            public const string EmailNotVerified = "Please verify your email before logging in.";
            public const string EmailAlreadyRegistered = "This email address is already registered.";
            public const string RegistrationFailed = "An error occurred while creating your account. Please try again.";
            public const string InvalidCurrentPassword = "Invalid current password.";
            public const string InvalidPasswordConfirm = "Invalid password confirmation.";
            public const string EmailInUse = "Email is already in use.";
            public const string LogoutFailed = "Logout failed. Please try again.";
            public const string ProfileUpdateFailed = "Profile update failed.";
            public const string PasswordResetFailed = "Password reset failed. Please try again.";
            public const string AccountLocked = "Account is locked. Try again in {0} minutes.";
            public const string AccountSuspended = "Your account is temporarily suspended. Please contact HR.";
            public const string AccountBanned = "Your account has been permanently banned.";
            public const string AccountPending = "Your account is pending approval from an administrator.";
        }

        // 2. Tokens & Security
        public static class Security
        {
            public const string InvalidRefreshToken = "Invalid or expired refresh token.";
            public const string Unauthorized = "You are not authorized to access this resource.";
            public const string UnauthorizedUserId = "Unauthorized: User identification missing in token.";
            public const string JwtSettingsMissing = "JwtSettings is missing in appsettings.json!";
        }

        // 3. OTP & Email Verification
        public static class Verification
        {
            public const string InvalidOtp = "Invalid verification code.";
            public const string OtpExpired = "Verification code has expired.";
            public const string PasswordResetUserNotFound = "User with this email was not found.";
            public const string VerifyEmailSubject = "Verify Your Account - YouTube Clone";
            public const string PasswordResetSubject = "Reset Your Password - YouTube Clone";
            public const string EmailChangeSubject = "Confirm Your New Email Address - YouTube Clone";
            public const string OtpNotFound = "Verification code not found.";
        }

        // 4. File Management
        public static class Files
        {
            public const string FileEmpty = "File is empty.";
            public const string FileSizeExceeded = "File size exceeds the 2MB limit.";
            public const string InvalidFileType = "Only {0} files are allowed.";
        }

        // 5. System & Infrastructure
        public static class System
        {
            public const string InternalServerError = "An unexpected error occurred. Please try again later.";
            public const string SeedSettingsMissing = "SeedSettings is missing in appsettings.json!";
            public const string EmailSettingsMissing = "EmailSettings is missing in appsettings.json!";
            public const string ValidationFailed = "Validation failed";
            public const string SeedingSuccess = "Database Seeding Successful!";
            public const string DatabaseSeedingError = "An error occurred while seeding the database.";
        }

        // 6. Success Messages
        public static class Success
        {
            public const string Login = "Logged in successfully.";
            public const string Logout = "Logged out successfully.";
            public const string Register = "Registration successful! Please check your email for the OTP code.";
            public const string ProfileUpdate = "Profile updated successfully!";
            public const string ProfilePicture = "Profile picture updated successfully.";
            public const string ProfilePictureRemoved = "Profile picture removed.";
            public const string PasswordUpdate = "Password updated successfully.";
            public const string EmailUpdate = "Email updated successfully.";
            public const string OtpVerify = "Email verified successfully! You can now login.";
            public const string ForgotPassword = "If your email exists in our system, a reset code has been sent.";
            public const string PasswordReset = "Your password has been reset successfully. You can now login.";
            public const string AccountDeactivated = "Account deactivated. You have 30 days to restore it.";
            public const string AccountDeleted = "Account permanently deleted.";
            public const string Authorized = "Authenticated successfully!";
            public const string EmailChangeRequestSent = "Verification code sent to your new email.";
        }

        // 7. Email Content Defaults
        public static class EmailDefaults
        {
            public const string EmailSendFailed = "Failed to send verification email.";
            public const string EmailSenderName = "YouTube Clone Support";
            public const string FromAddress = "mailtrap@demomailtrap.co";
        }

        public static class Infrastructure
        {
            public const string ConnectionStringNotFound = "Database connection string 'DefaultConnection' is missing in appsettings.json.";
        }
    }
}

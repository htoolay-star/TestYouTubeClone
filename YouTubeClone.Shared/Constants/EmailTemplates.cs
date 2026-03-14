using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.Constants
{
    public static class EmailTemplates
    {
        public const string OtpEmailBody = @"
            <div style='max-width: 500px; margin: 0 auto; font-family: Segoe UI, Tahoma; border: 1px solid #f0f0f0; border-radius: 12px; overflow: hidden;'>
                <div style='background-color: #FF0000; padding: 20px; text-align: center;'>
                    <h1 style='color: white; margin: 0;'>YouTube Clone</h1>
                </div>
                <div style='padding: 30px; text-align: center;'>
                    <h2>Verify Your Account</h2>
                    <p>Use the code below to complete your registration:</p>
                    <div style='margin: 30px auto; padding: 15px; background-color: #f9f9f9; border: 2px dashed #FF0000; border-radius: 8px; display: inline-block;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px;'>{0}</span>
                    </div>
                    <p style='color: #999;'>Valid for 15 minutes only.</p>
                </div>
            </div>";

        public const string PasswordResetBody = @"
            <div style='font-family: sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                <h2 style='color: #FF0000;'>YouTube Clone</h2>
                <h3>Password Reset Request</h3>
                <p>Please use the following code to reset your password:</p>
                <div style='background: #f4f4f4; padding: 10px; text-align: center; font-size: 24px; font-weight: bold;'>
                    {0}
                </div>
                <p>This code will expire in 15 minutes.</p>
            </div>";

        public const string EmailChangeBody = @"
            <div style='max-width: 500px; margin: 0 auto; font-family: Segoe UI, Tahoma; border: 1px solid #f0f0f0; border-radius: 12px; overflow: hidden;'>
                <div style='background-color: #FF0000; padding: 20px; text-align: center;'>
                    <h1 style='color: white; margin: 0;'>YouTube Clone</h1>
                </div>
                <div style='padding: 30px; text-align: center;'>
                    <h2>Email Change Request</h2>
                    <p>You requested to change your email. Use the code below to verify your <b>new email address</b>:</p>
                    <div style='margin: 30px auto; padding: 15px; background-color: #f9f9f9; border: 2px dashed #FF0000; border-radius: 8px; display: inline-block;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px;'>{0}</span>
                    </div>
                    <p style='color: #999;'>If you didn't request this, please ignore this email and secure your account.</p>
                    <p style='color: #999;'>Valid for 15 minutes only.</p>
                </div>
            </div>";
    }
}

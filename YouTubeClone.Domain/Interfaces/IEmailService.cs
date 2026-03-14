using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IEmailService
    {
        // to: လက်ခံမယ့်သူ၊ subject: ခေါင်းစဉ်၊ body: ပါဝင်မယ့် အကြောင်းအရာ
        Task SendEmailAsync(string to, string subject, string body);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Enums
{
    public enum AccountStatus : byte
    {
        Pending = 0,
        Active = 1,
        Suspended = 2,
        Banned = 3
    }
}

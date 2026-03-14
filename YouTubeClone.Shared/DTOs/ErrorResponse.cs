using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YouTubeClone.Shared.DTOs
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Errors { get; set; }
        public string? Details { get; set; } // Development မှာပဲ ပြမယ်

        public ErrorResponse(int statusCode, string message, object? errors = null, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
            Details = details;
        }
    }
}

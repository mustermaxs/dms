using Microsoft.AspNetCore.Mvc;

namespace DMS.Api.Configuration
{
    public class Response<T> : ActionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Content { get; set; }
    }
    
    public class Response : ActionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Content { get; } = null;
    }
}
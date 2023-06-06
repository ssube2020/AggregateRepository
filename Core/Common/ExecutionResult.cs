using System;
namespace Core.Common
{
    public class ExecutionResult<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? ErrorMessage { get; set; }

        public T? Data { get; set; }

        public int StatusCode { get; set; } = 200;
    }
}


﻿namespace CommandsReceiver.Application.Domain.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
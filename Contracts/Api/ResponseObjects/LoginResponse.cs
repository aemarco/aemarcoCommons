using System;

namespace Contracts.Api.ResponseObjects
{
    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTimeOffset TokenValidUntil { get; set; }
    }
}

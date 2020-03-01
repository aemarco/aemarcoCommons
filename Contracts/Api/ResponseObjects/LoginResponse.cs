using System;

namespace Contracts.Api.ResponseObjects
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTimeOffset TokenValidUntil { get; set; }
    }
}

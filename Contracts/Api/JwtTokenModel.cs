using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Api
{
    public class JwtTokenModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int AdultLevel { get; set; }
    }
}
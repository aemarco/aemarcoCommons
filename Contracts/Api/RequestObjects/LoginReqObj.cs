using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;


namespace Contracts.Api.RequestObjects
{
    public class LoginReqObj
    {

        [DataType(DataType.Password)]
        public string Token { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;

        public string WindowsUser { get; set; } = Environment.UserName;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;


namespace Contracts.Api.RequestObjects
{
    public class LoginReqObj
    {
        /// <summary>
        /// still valid token allows token renewal
        /// </summary>
        public string Token { get; set; }


        /// <summary>
        /// email of the user to issue a token for
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        /// <summary>
        /// password of the user to issue a token for
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// username which being used on client machine
        /// </summary>
        [Required]
        public string WindowsUser { get; set; } = Environment.UserName;







        [Obsolete]
        public bool RememberMe { get; set; }

        [Obsolete]
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

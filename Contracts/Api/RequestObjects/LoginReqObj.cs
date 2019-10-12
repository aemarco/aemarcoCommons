using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;


namespace Contracts.Api.RequestObjects
{
    /// <summary>
    /// ATTENTION: Keep in sync with 'Toolbox.ApiTools.RequestObjects'
    /// </summary>
    public class LoginReqObj
    {
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        public virtual bool RememberMe { get; set; } = false;

        public virtual string WindowsUser { get; set; } = Environment.UserName;

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

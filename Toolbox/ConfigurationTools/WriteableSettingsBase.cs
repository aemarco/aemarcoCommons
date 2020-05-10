using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Toolbox.ConfigurationTools
{
    public class WriteableSettingsBase : SettingsBase
    {
        [JsonIgnore]
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual string Filepath => throw new NotImplementedException();

        protected WriteableSettingsBase(IConfiguration root)
            : base(root)
        { }

        public void SaveChanges()
        {
            var sb = new StringBuilder();

            sb.Append($"{{\"{GetType().Name}\": ");
            sb.Append(JsonConvert.SerializeObject(this, Formatting.Indented));
            sb.Append("}");
            File.WriteAllText(Filepath, sb.ToString());
        }
    }
}

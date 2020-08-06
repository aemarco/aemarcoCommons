using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Toolbox.ConfigurationTools
{
    public abstract class WriteableSettingsBase : SettingsBase
    {

        //ensure derived class defines location to be saved to
        [JsonIgnore] 
        protected abstract string Filepath { get; }
    
        protected WriteableSettingsBase(IConfiguration root)
            : base(root)
        { }

        //ensure Placeholders getting ignored
        protected override void HandleStringPlaceholders(IConfiguration root) { }


        public virtual void SaveChanges()
        {
            if (string.IsNullOrWhiteSpace(Filepath)) return;


            var sb = new StringBuilder();
            sb.Append($"{{\"{GetType().Name}\": ");
            sb.Append(JsonConvert.SerializeObject(this, Formatting.Indented));
            sb.Append("}");
            File.WriteAllText(Filepath, sb.ToString());
        }
    }
}

using System;
using System.Reflection;
using aemarcoCommons.Toolbox.CryptoTools;
using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.Toolbox.Autofac.AppConfiguration.Transformations
{

    /// <summary>
    /// Use this to En-/Decrypt string properties while Saving and Loading
    /// You need to add the ProtectedTransformer during setup
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ProtectedAttribute : Attribute { }




    public class ProtectedTransformer : StringTransformerBase
    {
        private readonly string _password;
        public ProtectedTransformer(string password)
        {
            _password = password;
        }


        public override string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
        {
            //password must be specified
            var _ = _password ?? throw new ArgumentException("No Password provided for Cryptography");


            //skip if not a protected string, otherwise decrypt
            return Attribute.IsDefined(propertyInfo, typeof(ProtectedAttribute)) 
                ? PasswordTextCipher.Decrypt(currentValue, _password)
                : currentValue;
        }

        public override string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
        {
            //password must be specified
            var _ = _password ?? throw new ArgumentException("No Password provided for Cryptography");


            //skip if not a protected string, otherwise decrypt
            return Attribute.IsDefined(propertyInfo, typeof(ProtectedAttribute)) 
                ? PasswordTextCipher.Encrypt(currentValue, _password)
                : currentValue;
        }
    }
}

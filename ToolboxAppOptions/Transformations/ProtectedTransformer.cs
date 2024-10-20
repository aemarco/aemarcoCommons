using aemarcoCommons.Extensions.CryptoExtensions;

namespace aemarcoCommons.ToolboxAppOptions.Transformations;

/// <summary>
/// Use this to En-/Decrypt string properties while Saving and Loading
/// You need to add the ProtectedTransformer during setup
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ProtectedAttribute : Attribute;



/// <summary>
/// This Transformer decrypts config strings during read, and encrypts them during write
/// To use this, add it in the StringTransformations List, inside the AddConfigurationUtils options
/// </summary>
public class ProtectedTransformer : StringTransformerBase
{
    private readonly string _password;
    public ProtectedTransformer(
        string password)
    {
        _password = password;
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
    }

    public override string PerformReadTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
    {
        //only those marked "Protected"
        if (!Attribute.IsDefined(propertyInfo, typeof(ProtectedAttribute)))
            return currentValue;

        //don´t attempt to decrypt empty strings
        if (string.IsNullOrWhiteSpace(currentValue))
            return currentValue;

        //decrypt
        return currentValue.DecryptFromBase64(_password);
    }

    public override string PerformWriteTransformation(string currentValue, PropertyInfo propertyInfo, IConfigurationRoot configRoot)
    {
        //only those marked "Protected"
        if (!Attribute.IsDefined(propertyInfo, typeof(ProtectedAttribute)))
            return currentValue;

        //don´t attempt to encrypt empty strings
        if (string.IsNullOrWhiteSpace(currentValue))
            return currentValue;

        //encrypt
        return currentValue.EncryptToBase64(_password);
    }
}

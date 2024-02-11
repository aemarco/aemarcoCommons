using System;
using System.Windows.Markup;

namespace aemarcoCommons.WpfTools.MarkupExtensions;

//simplified
//https://youtu.be/Bp5LFXjwtQ0

public class EnumBindingSourceExtension : MarkupExtension
{

    private readonly Type _enumType;
    public EnumBindingSourceExtension(Type enumType)
    {
        if (enumType is null || !enumType.IsEnum)
            throw new ArgumentException("Type must be for an Enum.", nameof(enumType));

        _enumType = enumType;
    }


    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(_enumType);

        //if (_enumType == null)
        //    throw new InvalidOperationException("The EnumType must be specified.");

        //var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
        //var enumValues = Enum.GetValues(actualEnumType);

        //if (actualEnumType == _enumType)
        //    return enumValues;

        //var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
        //enumValues.CopyTo(tempArray, 1);
        //return tempArray;
    }
}
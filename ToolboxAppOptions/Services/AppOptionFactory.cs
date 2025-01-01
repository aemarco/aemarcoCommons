namespace aemarcoCommons.ToolboxAppOptions.Services;

public class AppOptionFactory<TOptions> :
    IConfigureOptions<TOptions>,
    IPostConfigureOptions<TOptions>,
    IValidateOptions<TOptions>
    where TOptions : class, ISettingsBase
{

    private readonly IConfigurationRoot _config;
    private readonly ToolboxAppOptionsSettings _options;
    private readonly IValidator<TOptions>? _validator;
    public AppOptionFactory(
        IConfigurationRoot config,
        ToolboxAppOptionsSettings options,
        IValidator<TOptions>? validator = null)
    {
        _config = config;
        _options = options;
        _validator = validator;
    }

    //IConfigureOptions
    public void Configure(TOptions options)
    {
        var type = typeof(TOptions);
        var path = type.GetAttribute<SettingsPathAttribute>() is { } pathAttribute
            ? pathAttribute.Path
            : type.Name;

        if (string.IsNullOrWhiteSpace(path))
            _config.Bind(options);
        else
            _config.GetSection(path).Bind(options);
    }

    //IPostConfigureOptions
    public void PostConfigure(string? name, TOptions options)
    {
        //ApplyReadTransformations
        foreach (var transformation in _options.StringTransformations)
            StringTransformerBase.TransformObject(options, _config, transformation.PerformReadTransformation);
    }

    //IValidateOptions
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        //inspired by Nick
        //https://youtu.be/jblRYDMTtvg


        // Ensure options are provided to validate against
        ArgumentNullException.ThrowIfNull(nameof(options));


        //if no validator is defined, we are fine
        if (_validator is null)
            return ValidateOptionsResult.Success;

        //validate
        var valRes = _validator.Validate(options);
        return valRes.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(valRes.Errors.Select(x =>
                $"Options validation failed for '{x.PropertyName}' with error: '{x.ErrorMessage}'"));
    }

}
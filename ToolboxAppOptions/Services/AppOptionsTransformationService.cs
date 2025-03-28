﻿namespace aemarcoCommons.ToolboxAppOptions.Services;

public class AppOptionsTransformationService
{

    private readonly ToolboxAppOptionsSettings _options;
    private readonly IConfigurationRoot _config;
    public AppOptionsTransformationService(
        ToolboxAppOptionsSettings options,
        IConfigurationRoot config)
    {
        _options = options;
        _config = config;
    }


    public void PerformWriteTransformation(ISettingsBase options)
    {
        var transformations = _options.StringTransformations.ToList();
        transformations.Reverse();

        //ApplyReadTransformations
        foreach (var transformation in transformations)
            StringTransformerBase.TransformObject(options, _config, transformation.PerformWriteTransformation);
    }

}

using System;
using System.IO;

namespace aemarcoCommons.Toolbox.Autofac.AppConfiguration
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Gather absolute file Path which the type get saved to
        /// </summary>
        /// <param name="type">type for which the file Path should be returned</param>
        /// <param name="saveDirectory">directory path (same as provided in options)</param>
        /// <returns>absolute file path to the saved configuration file</returns>
        internal static string GetSavePathForSetting(this Type type, string saveDirectory)
        {
            _ = saveDirectory ?? throw new ApplicationException("Saving disabled. To enable don´t set SettingsSaveDirectory to null");
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);


            var fileMiddleName = type.Name;
            //use path defined in attribute if specified
            if (Attribute.GetCustomAttribute(type, typeof(SettingPathAttribute)) is SettingPathAttribute pathAttribute &&
                !string.IsNullOrWhiteSpace(pathAttribute.Path))
                fileMiddleName = pathAttribute.Path.Replace(':', '_');
            var fileName = $"savedSettings.{fileMiddleName}.json";

            return string.IsNullOrWhiteSpace(saveDirectory)
                ? fileName
                : Path.Combine(saveDirectory, fileName);
        }

    }
}
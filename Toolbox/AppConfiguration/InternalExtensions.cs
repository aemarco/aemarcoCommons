using System;
using System.IO;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Gather absolute file Path which the type get saved to
        /// </summary>
        /// <param name="type">type for which the file Path should be returned</param>
        /// <param name="options">options</param>
        /// <returns>absolute file path to the saved configuration file</returns>
        internal static string GetSavePathForSetting(this Type type, ConfigurationOptions options)
        {
            var getFileName = options.GetFileNameForSettingsClass
                              ?? throw new ApplicationException("Saving disabled. To enable don´t set GetFileNameForSettingsClass to null");
            var saveDirectory = options.SettingsSaveDirectory
                                ?? throw new ApplicationException("Saving disabled. To enable don´t set SettingsSaveDirectory to null");
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);

            return Path.Combine(saveDirectory, getFileName(type));
        }

    }
}
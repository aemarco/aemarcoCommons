﻿using Microsoft.Win32;

namespace aemarcoCommons.Toolbox.RegistryTools
{
    public static class RegistryEditor
    {

        /// <summary>
        /// Reading a Registry Value
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        /// <returns>object or null</returns>
        public static object GetUserValue(string path, string key)
        {
            object result;
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path);
            try
            {
                result = reg?.GetValue(key);
            }
            finally
            {
                reg?.Close();
            }
            return result;
        }

        /// <summary>
        /// Setting a Registry Value
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        /// <param name="value">value</param>
        public static void SetUserValue(string path, string key, object value)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                reg?.SetValue(key, value);

            }
            finally
            {
                reg?.Close();
            }
        }


        /// <summary>
        /// Deletes a key/value pair in Registry
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        public static void DeleteUserValue(string path, string key)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                reg?.DeleteValue(key);

            }
            finally
            {
                reg?.Close();
            }

        }
    }
}

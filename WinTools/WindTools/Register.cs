
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace WinTools.WindTools
{
    public class Register
    {
        #region fields

        private const string AUTOSTARTPATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private readonly string _publisherName;
        private readonly string _appName;
        private readonly string _apprefLocation;

        #endregion

        #region ctor

        public Register()
        {
            //appname needed for autostart
            _appName = Deployment.Product ?? Process.GetCurrentProcess().ProcessName;
            _publisherName = Deployment.Publisher ?? "aemarco";

            //location needed for autostart
            _apprefLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\{_publisherName}\\" +
                $"{_appName}.appref-ms";


            //auto-change of autostart path based on key
            EnsureCorrectPathInAutostart();
        }

        private void EnsureCorrectPathInAutostart()
        {
            string path = GetValue(_appName, AUTOSTARTPATH);
            if (path != null && path != _apprefLocation)
            {
                SetValue(_appName, AUTOSTARTPATH, _apprefLocation);
            }
        }

        #endregion

        #region props

        public bool AppAutostarted
        {
            get
            {
                var path = GetValue(_appName, AUTOSTARTPATH);
                return path != null && path == _apprefLocation;
            }
            set
            {

                switch (value)
                {
                    case false:
                        DeleteValue(_appName, AUTOSTARTPATH);
                        break;
                    case true:
                        SetValue(_appName, AUTOSTARTPATH, _apprefLocation);
                        break;
                }

            }
        }


        #endregion

        #region "read/set value in Registry"

        /// <summary>
        /// Reading a Registry Value
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        /// <returns>string value or null</returns>
        public string GetValue(string key, string path)
        {
            string result = null;
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path);
            try
            {
                result = reg?.GetValue(key)?.ToString();
            }
            finally
            {
                reg.Close();
            }
            return result;
        }

        /// <summary>
        /// Setting a Registry Value
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        /// <param name="value">string value</param>
        public void SetValue(string key, string path, string value)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                reg.SetValue(key, value);

            }
            finally
            {
                reg.Close();
            }
        }

        /// <summary>
        /// Deletes a key/value pair in Registry
        /// </summary>
        /// <param name="key">key for the value</param>
        /// <param name="path">path of the registry</param>
        public void DeleteValue(string key, string path)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                reg.DeleteValue(key);

            }
            finally
            {
                reg.Close();
            }

        }

        #endregion


    }
}

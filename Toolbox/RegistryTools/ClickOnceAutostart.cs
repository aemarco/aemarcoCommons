using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.RegistryTools
{
    public class ClickOnceAutostart
    {
        #region ctor

        private readonly string _publisherName;
        private readonly string _appName;

        private const string AUTOSTARTPATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private readonly string _apprefLocation;

        public ClickOnceAutostart(string publisherName, string appName)
        {
            if (string.IsNullOrWhiteSpace(publisherName))
                throw new ArgumentException("Publisher must be known", nameof(publisherName));
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentException("Appname must be known", nameof(appName));


            //appname needed for autostart
            _publisherName = publisherName;
            _appName = appName;


            //location needed for autostart
            _apprefLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\{_publisherName}\\" +
                $"{_appName}.appref-ms";

            //auto-change of autostart path based on key
            EnsureCorrectPathInAutostart();
        }

        private void EnsureCorrectPathInAutostart()
        {
            string path = RegistryEditor.GetUserValue(AUTOSTARTPATH, _appName)?.ToString();
            if (path != null && path != _apprefLocation)
            {
                RegistryEditor.SetUserValue(AUTOSTARTPATH, _appName, _apprefLocation);
            }
        }

        #endregion

        #region props

        public bool AppAutostarted
        {
            get
            {
                var path = RegistryEditor.GetUserValue(AUTOSTARTPATH, _appName)?.ToString();
                return path != null && path == _apprefLocation;
            }
            set
            {

                switch (value)
                {
                    case false:
                        RegistryEditor.DeleteUserValue(AUTOSTARTPATH, _appName);
                        break;
                    case true:
                        RegistryEditor.SetUserValue(AUTOSTARTPATH, _appName, _apprefLocation);
                        break;
                }

            }
        }


        #endregion




    }
}

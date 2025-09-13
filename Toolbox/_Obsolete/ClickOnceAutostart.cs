using System;
using System.Runtime.Versioning;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.RegistryTools
#pragma warning restore IDE0130
{
    [Obsolete("Will be removed in next major")]
    [SupportedOSPlatform("windows")]
    public class ClickOnceAutostart
    {

        private string _appName;
        public string AppName
        {
            get => _appName;
            set
            {
                if (value == _appName) return;

                _appName = value;
                UpdateAppRef();
            }
        }


        private string _publisher;
        public string Publisher
        {
            get => _publisher;
            set
            {
                if (value == _publisher) return;


                _publisher = value;
                UpdateAppRef();
            }
        }


        private bool _valid;
        private const string AutoStartPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private string _appRefLocation;
        private void UpdateAppRef()
        {
            if (string.IsNullOrWhiteSpace(_appName) ||
                string.IsNullOrWhiteSpace(_publisher)) return;


            //location needed for auto-start
            _appRefLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\" +
                              $"{_publisher}\\{_appName}.appref-ms";
            _valid = true;

            //auto-change of autostart path based on key
            var path = RegistryEditor.GetUserValue(AutoStartPath, _appName)?.ToString();
            if (path != null && path != _appRefLocation)
            {
                RegistryEditor.SetUserValue(AutoStartPath, _appName, _appRefLocation);
            }
        }

        public bool AppAutostarted
        {
            get
            {
                if (!_valid) return false;

                var path = RegistryEditor.GetUserValue(AutoStartPath, _appName)?.ToString();
                return path != null && path == _appRefLocation;
            }
            set
            {
                if (!_valid) return;

                switch (value)
                {
                    case false:
                        RegistryEditor.DeleteUserValue(AutoStartPath, _appName);
                        break;
                    case true:
                        RegistryEditor.SetUserValue(AutoStartPath, _appName, _appRefLocation);
                        break;
                }

            }
        }
    }
}

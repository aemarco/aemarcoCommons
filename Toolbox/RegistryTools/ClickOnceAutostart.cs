using System;
using Contracts.Interfaces;

namespace Toolbox.RegistryTools
{
    public class ClickOnceAutostart : ISingleton
    {
        
        private string _appName;
        public string AppName
        {
            get => _appName;
            set
            {
                if (value == _appName) return;

                _appName = value;
                UpdateAppref();
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
                UpdateAppref();
            }
        }


        private bool _valid = false;
        private const string AutostartPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private string _apprefLocation;
        private void UpdateAppref()
        {
            if (string.IsNullOrWhiteSpace(_appName) ||
                string.IsNullOrWhiteSpace(_publisher)) return;


            //location needed for autostart
            _apprefLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\" +
                              $"{_publisher}\\{_appName}.appref-ms";
            _valid = true;

            //auto-change of autostart path based on key
            var path = RegistryEditor.GetUserValue(AutostartPath, _appName)?.ToString();
            if (path != null && path != _apprefLocation)
            {
                RegistryEditor.SetUserValue(AutostartPath, _appName, _apprefLocation);
            }
        }

        public bool AppAutostarted
        {
            get
            {
                if (!_valid) return false;

                var path = RegistryEditor.GetUserValue(AutostartPath, _appName)?.ToString();
                return path != null && path == _apprefLocation;
            }
            set
            {
                if (!_valid) return;

                switch (value)
                {
                    case false:
                        RegistryEditor.DeleteUserValue(AutostartPath, _appName);
                        break;
                    case true:
                        RegistryEditor.SetUserValue(AutostartPath, _appName, _apprefLocation);
                        break;
                }

            }
        }
    }
}

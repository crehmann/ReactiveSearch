using Foundation;

namespace ReactiveSearch.iOS
{
    internal class ConfigProvider : IConfigProvider
    {
        private readonly string _configBundlePath;
        public ConfigProvider()
        {
            _configBundlePath = NSBundle.MainBundle.PathForResource("Config", "plist");
        }

        public string AppCenterApiToken => $"ios={NSDictionary.FromFile(_configBundlePath)["app_center_api_token"]};";
    }
}
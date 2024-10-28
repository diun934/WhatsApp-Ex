using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

//配置项操作类

namespace all_on_whatsapp
{
    public static class ConfigManager
    {
        private readonly static string _browserConfigFilePath = "Config/BrowserConfig.json";

        public static async Task<ObservableCollection<BrowserGroup>> LoadBrowserConfigAsync()
        {
            ObservableCollection<BrowserGroup> instantiations = new ObservableCollection<BrowserGroup>();

            try
            {
                if (!File.Exists(_browserConfigFilePath))
                {
                    Logger.Error($"File not found: {_browserConfigFilePath}");
                    return instantiations;
                }

                string jsonString = await File.ReadAllTextAsync(_browserConfigFilePath);
                //Debug.WriteLine(jsonString);

                if (string.IsNullOrEmpty(jsonString))
                {
                    Logger.Error("The instance list is empty!");
                    return instantiations;
                }

                var deserializedList = JsonConvert.DeserializeObject<List<BrowserGroup>>(jsonString);
                if (deserializedList != null)
                {
                    instantiations = new ObservableCollection<BrowserGroup>(deserializedList);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load instantiations: {ex.Message}");
            }

            return instantiations;
        }

        public static async Task SaveBrowserConfigAsync(ObservableCollection<BrowserGroup> browserGroup)
        {
            try
            {
                // 确保 Config 文件夹存在
                string? directoryPath = Path.GetDirectoryName(_browserConfigFilePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 序列化并保存配置文件
                string jsonData = JsonConvert.SerializeObject(browserGroup, Formatting.Indented);
                await File.WriteAllTextAsync(_browserConfigFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save the configuration file of the browser instance: {ex.Message}");
            }
        }

        private readonly static string _appConfigFilePath = "Config/AppConfig.json";
        public static async Task<Appsetings> LoadAppConfigAsync()
        {
            Appsetings configs = new Appsetings();

            try
            {
                if (!File.Exists(_appConfigFilePath))
                {
                    Logger.Error($"File not found: {_appConfigFilePath}");
                    return configs;
                }

                string jsonString = await File.ReadAllTextAsync(_appConfigFilePath);
                //Debug.WriteLine(jsonString);

                if (string.IsNullOrEmpty(jsonString))
                {
                    Logger.Error("The instance list is empty!");
                    return configs;
                }

                var deserializedList = JsonConvert.DeserializeObject<Appsetings>(jsonString);
                if (deserializedList != null)
                {
                    configs = deserializedList;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load instantiations: {ex.Message}");
            }

            return configs;
        }

        public static async Task SaveAppConfigAsync(Appsetings configs)
        {
            try
            {
                // 确保 Config 文件夹存在
                string? directoryPath = Path.GetDirectoryName(_appConfigFilePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string jsonData = JsonConvert.SerializeObject(configs, Formatting.Indented);
                await File.WriteAllTextAsync(_appConfigFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save the configuration file of the browser instance: {ex.Message}");
            }
        }

        private readonly static string _pinUserList = "Config/PinUserList.json";

        public static async Task<ObservableCollection<PinUserModel>> LoadPinUserListAsync()
        {
            ObservableCollection<PinUserModel> configs = new ObservableCollection<PinUserModel>();

            try
            {
                if (!File.Exists(_pinUserList))
                {
                    Logger.Error($"File not found: {_pinUserList}");
                    return configs;
                }

                string jsonString = await File.ReadAllTextAsync(_pinUserList);
                //Debug.WriteLine(jsonString);

                if (string.IsNullOrEmpty(jsonString))
                {
                    Logger.Error("The instance list is empty!");
                    return configs;
                }

                var deserializedList = JsonConvert.DeserializeObject<ObservableCollection<PinUserModel>>(jsonString);
                if (deserializedList != null)
                {
                    configs = deserializedList;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load instantiations: {ex.Message}");
            }

            return configs;
        }

        public static async Task SavePinUserListAsync(ObservableCollection<PinUserModel> configs)
        {
            try
            {
                // 确保 Config 文件夹存在
                string? directoryPath = Path.GetDirectoryName(_pinUserList);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string jsonData = JsonConvert.SerializeObject(configs, Formatting.Indented);
                await File.WriteAllTextAsync(_pinUserList, jsonData);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save the configuration file of the browser instance: {ex.Message}");
            }
        }
    }
}

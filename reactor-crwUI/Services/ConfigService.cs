using Microsoft.Win32;

using reactor_crwUI.Model;
using reactor_crwUI.Services.Interfaces;

using System;
using System.IO;
using System.Text.Json;

namespace reactor_crwUI.Services
{
    class ConfigService : IConfigService
    {
        private const string _chooseDestinationFolderMessage = "Выберите папку, в которую будет сохранён файл конфигурации.";
        private const string _defFileName = "settings";
        private const string _defExt = ".json";
        private const string _dlgFilter = "Json documents (.json)|*.json";
        public bool SaveConfig(Config config)
        {
            try
            {
                var outputPath = string.Empty;
                var dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.CreatePrompt = true;
                dialog.DefaultExt = _defExt;
                dialog.FileName = _defFileName;
                dialog.OverwritePrompt = true;
                dialog.Title = _chooseDestinationFolderMessage;
                dialog.Filter = _dlgFilter;

                var result = dialog.ShowDialog();
                if (result is true)
                {
                    outputPath = dialog.FileName;
                    var jsonString = JsonSerializer.Serialize(config);
                    File.WriteAllText(outputPath, jsonString);
                }

                return File.Exists(outputPath);
            }
            catch(Exception ex)
            {
                throw new Exception("Непредвиденная ошибка!", ex);
            }
        }

        public Config LoadConfig()
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.DefaultExt = _defExt;
                dlg.Filter = _dlgFilter;
                var result = dlg.ShowDialog();
                var fileName = string.Empty;

                if (result is true)
                {
                    fileName = dlg.FileName;
                    var jsonString = File.ReadAllText(fileName);
                    var config = JsonSerializer.Deserialize<Config>(jsonString);
                    return config;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно прочитать файл настроек!", ex);
            }
        }
    }
}

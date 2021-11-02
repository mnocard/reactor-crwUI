using reactor_crwUI.Core;
using reactor_crwUI.Model;
using reactor_crwUI.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace reactor_crwUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelCore
    {
        public MainWindowViewModel(IConfigService ConfigService)
        {
            _ConfigService = ConfigService;

            #region Команды
            RCRWPathCommand = new LambdaCommand(OnRCRWPathCommandExecuted, CanRCRWPathCommandExecute);
            OutputPathCommand = new LambdaCommand(OnOutputPathCommandExecuted, CanOutputPathCommandExecute);
            StartCrawlCommand = new LambdaCommand(OnStartCrawlCommandExecuted, CanStartCrawlCommandExecute);
            LoadConfigCommand = new LambdaCommand(OnLoadConfigCommandExecuted, CanLoadConfigCommandExecute);
            SaveConfigCommand = new LambdaCommand(OnSaveConfigCommandExecuted, CanSaveConfigCommandExecute);

            #endregion
        }

        #region Свойста

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "reactor_crwUI";

        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #region RCRWPath : string - Полное расположение reactor-crw

        /// <summary>Полное расположение reactor-crw</summary>
        private string _RCRWPath;

        /// <summary>Полное расположение reactor-crw</summary>
        public string RCRWPath
        {
            get => _RCRWPath;
            set => Set(ref _RCRWPath, value);
        }

        #endregion

        #region CookiesAccepted : bool - Использовать печеньки

        /// <summary>Использовать печеньки</summary>
        private bool _CookiesAccepted = false;

        /// <summary>Использовать печеньки</summary>
        public bool CookiesAccepted
        {
            get => _CookiesAccepted;
            set => Set(ref _CookiesAccepted, value);
        }

        #endregion

        #region CookiesData : string - Собственно печенька

        /// <summary>Собственно печенька</summary>
        private string _CookiesData;

        /// <summary>Собственно печенька</summary>
        public string CookiesData
        {
            get => _CookiesData;
            set => Set(ref _CookiesData, value);
        }

        #endregion

        #region OutputPath : string - Папка, в которую будет загружен скачанные контент

        /// <summary>Папка, в которую будет загружен скачанные контент</summary>
        private string _OutputPath;

        /// <summary>Папка, в которую будет загружен скачанные контент</summary>
        public string OutputPath
        {
            get => _OutputPath;
            set => Set(ref _OutputPath, value);
        }
        #endregion

        #region URL : string - Адрес сайта

        /// <summary>Адрес сайта</summary>
        private string _URL;

        /// <summary>Адрес сайта</summary>
        public string URL
        {
            get => _URL;
            set => Set(ref _URL, value);
        }

        #endregion

        #region CorrectUrl : bool - Правильно ли сформирован адрес сайта

        /// <summary>Правильно ли сформирован адрес сайта</summary>
        private bool _CorrectUrl;

        /// <summary>Правильно ли сформирован адрес сайта</summary>
        public bool CorrectUrl
        {
            get => _CorrectUrl;
            set => Set(ref _CorrectUrl, value);
        }
        #endregion

        #region Status : string - Текущий статус программы

        /// <summary>Текущий статус программы</summary>
        private string _Status = string.Empty;

        /// <summary>Текущий статус программы</summary>
        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        #endregion

        #region Типы изображений

        #region ImageTypes : Dictionary<string, bool> - Словарь форматов контента для поиска, где string - название формата, bool - необходимость его поиска

        /// <summary>Словарь форматов контента для поиска, где string - название формата, bool - необходимость его загрузки</summary>
        public Dictionary<string, bool> _ImageTypes = new()
        {
            { "image", true },
            { "gif", true },
            { "webm", true },
            { "mp4", true },
        };

        #endregion

        #region ImageCheckBox : bool - Выбирать изображения

        /// <summary>Выбирать изображения</summary>
        public bool ImageCheckBox
        {
            get => _ImageTypes["image"];
            set
            {
                _ImageTypes["image"] = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region GifCheckBox : bool - Выбрать гифки

        /// <summary>Выбрать гифки</summary>
        public bool GifCheckBox
        {
            get => _ImageTypes["gif"];
            set
            {
                _ImageTypes["gif"] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region WebmCheckBox : bool - Выбрать webm
        /// <summary>Выбрать webm</summary>
        public bool WebmCheckBox
        {
            get => _ImageTypes["webm"];
            set
            {
                _ImageTypes["webm"] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Mp4CheckBox : bool - Выбрать mp4
        /// <summary>Выбрать mp4</summary>
        public bool Mp4CheckBox
        {
            get => _ImageTypes["mp4"];
            set
            {
                _ImageTypes["mp4"] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region OnePage : bool - Загружать только одну страницу

        /// <summary>Загружать только одну страницу</summary>
        private bool _OnePage;

        /// <summary>Загружать только одну страницу</summary>
        public bool OnePage
        {
            get => _OnePage;
            set => Set(ref _OnePage, value);
        }
        #endregion

        #region NumOfWorkers : int - Приоритет загрузки (workers)

        /// <summary>Приоритет загрузки (workers)</summary>
        private int _NumOfWorkers = 1;

        /// <summary>Приоритет загрузки (workers)</summary>
        public int NumOfWorkers
        {
            get => _NumOfWorkers;
            set
            {
                if (value < 1)
                    Set(ref _NumOfWorkers, 1);
                else if (value > 4)
                    Set(ref _NumOfWorkers, 4);
                else
                    Set(ref _NumOfWorkers, value);
            }
        }

        #endregion

        #endregion

        #region Команды

        #region Указание расположения reactor-crw
        /// <summary>Указание расположения reactor-crw</summary>
        public ICommand RCRWPathCommand { get; }
        /// <summary>Указание расположения reactor-crw</summary>
        private void OnRCRWPathCommandExecuted(object parameter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = _defExt;
            dlg.Filter = _dlgFilter;
            var result = dlg.ShowDialog();

            if (result == true)
            {
                RCRWPath = dlg.FileName;
            }
        }

        private bool CanRCRWPathCommandExecute(object parameter) => true;

        #endregion

        #region Указание расположения загрузки контента
        /// <summary>Указание расположения загрузки контента</summary>
        public ICommand OutputPathCommand { get; }


        /// <summary>Указание расположения загрузки контента</summary>
        private void OnOutputPathCommandExecuted(object parameter)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = _chooseDestinationFolderMessage;
                dialog.ShowNewFolderButton = true;
                dialog.AutoUpgradeEnabled = true;
                dialog.UseDescriptionForTitle = true;
                var result = dialog.ShowDialog();
                if(result is DialogResult.OK)
                {
                    OutputPath = dialog.SelectedPath;
                }
            }
        }
        private bool CanOutputPathCommandExecute(object parameter) => true;

        #endregion

        #region Начать загрузку контента
        /// <summary>Начать загрузку контента</summary>
        public ICommand StartCrawlCommand { get; }

        /// <summary>Начать загрузку контента</summary>
        private void OnStartCrawlCommandExecuted(object parameter)
        {
            CorrectUrl = Uri.IsWellFormedUriString(URL, UriKind.Absolute);
            
            var imgTypesSelected = _ImageTypes.Any(v => v.Value);
            if (!imgTypesSelected)
                Status += _noItemsSelected;

            if (!CorrectUrl)
                Status += _uriErrorMessage;

            if (CorrectUrl && imgTypesSelected)
            {
                var args = BuildArgForCrawler();
                Status += "\n" + args;
                Process.Start("cmd", "/c START " + args);
            }
        }
        private bool CanStartCrawlCommandExecute(object parameter) => true;

        #endregion

        #region Загрузка файла конфигурации
        /// <summary>Загрузка файла конфигурации</summary>
        public ICommand LoadConfigCommand { get; }
        /// <summary>Загрузка файла конфигурации</summary>
        private void OnLoadConfigCommandExecuted(object parameter)
        {
            try
            {
                var config = _ConfigService.LoadConfig();
                if (config is not null)
                {
                    RCRWPath = config.RCRWPath;
                    CookiesAccepted = config.CookiesAccepted;
                    CookiesData = config.CookiesData;
                    OutputPath = config.OutputPath;
                    URL = config.URL;
                    OnePage = config.OnePage;
                    NumOfWorkers = config.NumOfWorkers;

                    ImageCheckBox = config.ImageTypes["image"];
                    WebmCheckBox = config.ImageTypes["webm"];
                    GifCheckBox = config.ImageTypes["gif"];
                    Mp4CheckBox = config.ImageTypes["mp4"];
                }
            }
            catch (Exception ex)
            {
                Status = "Непредвиденная ошибка!\n" + ex.InnerException.Message;
            }
        }

        private bool CanLoadConfigCommandExecute(object parameter) => true;

        #endregion

        #region Сохранение файла конфигурации
        /// <summary>Сохранение файла конфигурации</summary>
        public ICommand SaveConfigCommand { get; }
        /// <summary>Сохранение файла конфигурации</summary>
        private void OnSaveConfigCommandExecuted(object parameter)
        {
            Config config = new()
            {
                RCRWPath = RCRWPath,
                CookiesAccepted = CookiesAccepted,
                CookiesData = CookiesData,
                OutputPath = OutputPath,
                URL = URL,
                OnePage = OnePage,
                NumOfWorkers = NumOfWorkers,
                ImageTypes = new (_ImageTypes),
            };

            try
            {
                var result = _ConfigService.SaveConfig(config);
                if (result) Status = "Настройки успешно сохранены.";
                else Status = "Что-то пошло не так.";
            }
            catch (Exception ex)
            {
                Status = "Непредвиденная ошибка!\n" + ex.InnerException.Message;
            }
        }

        private bool CanSaveConfigCommandExecute(object parameter) => true;

        #endregion

        #endregion

        #region Константы
        private const string _defExt = ".exe";
        private const string _dlgFilter = "Исполняемые файлы (.exe)|*.exe";
        private const string _uriErrorMessage = "\nОшибка в адресе сайта.";
        private const string _chooseDestinationFolderMessage = "Выберите папку, в которую будет загружен контент";
        private const string _noItemsSelected = "\nНе выбрано ни одного типа контента.";

        #endregion

        #region Сервисы
        private readonly IConfigService _ConfigService;
        #endregion

        #region Приватные методы

        /// <summary>
        /// Создание строки аргументов для передачи в краулер
        /// </summary>
        /// <returns>Получившаяся строка</returns>
        private string BuildArgForCrawler()
        {
            string args = string.Empty;
            var str = new StringBuilder();
            var imgTypes = new StringBuilder();

            foreach (var item in _ImageTypes.Where(v => v.Value))
            {
                imgTypes.Append(item.Key);
                imgTypes.Append(",");
            }
            imgTypes.Remove(imgTypes.Length - 1, 1);

            str = str
                .Append(RCRWPath)
                .Append(" -p ")
                .Append("\"")
                .Append(URL)
                .Append("\"")
                .Append(" -s ")
                .Append("\"")
                .Append(imgTypes)
                .Append("\"")
                .Append(" -w ")
                .Append(NumOfWorkers);

            if (!string.IsNullOrEmpty(OutputPath))
                str.Append(" -d ")
                .Append("\"")
                .Append(OutputPath)
                .Append("\"");

            if (CookiesAccepted && !string.IsNullOrEmpty(CookiesData))
                str.Append(" -c ")
                .Append("\"")
                .Append(CookiesData)
                .Append("\"");

            if (OnePage)
                str.Append(" -o ");

            return str.ToString();
        }
        #endregion
    }
}

using reactor_crwUI.Core;
using reactor_crwUI.Model;
using reactor_crwUI.Services.Interfaces;

using Serilog;

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
            ClosingAppCommand = new LambdaCommand(OnClosingAppCommandExecuted, CanClosingAppCommandExecute);

            #endregion
        }

        #region Свойста

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = _recator_crwUI;

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
            { _image, true },
            { _gif, true },
            { _webm, true },
            { _mp4, true },
        };

        #endregion

        #region ImageCheckBox : bool - Выбирать изображения

        /// <summary>Выбирать изображения</summary>
        public bool ImageCheckBox
        {
            get => _ImageTypes[_image];
            set
            {
                _ImageTypes[_image] = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region GifCheckBox : bool - Выбрать гифки

        /// <summary>Выбрать гифки</summary>
        public bool GifCheckBox
        {
            get => _ImageTypes[_gif];
            set
            {
                _ImageTypes[_gif] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region WebmCheckBox : bool - Выбрать webm
        /// <summary>Выбрать webm</summary>
        public bool WebmCheckBox
        {
            get => _ImageTypes[_webm];
            set
            {
                _ImageTypes[_webm] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Mp4CheckBox : bool - Выбрать mp4
        /// <summary>Выбрать mp4</summary>
        public bool Mp4CheckBox
        {
            get => _ImageTypes[_mp4];
            set
            {
                _ImageTypes[_mp4] = value;
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

        #region Закрытие приложения
        /// <summary>Закрытие приложения</summary>
        public ICommand ClosingAppCommand { get; }
        /// <summary>Закрытие приложения</summary>
        private void OnClosingAppCommandExecuted(object parameter)
        {
            _log.Information("Закрытие программы.");
            Log.CloseAndFlush();
        }

        private bool CanClosingAppCommandExecute(object parameter) => true;

        #endregion

        #region Указание расположения reactor-crw
        /// <summary>Указание расположения reactor-crw</summary>
        public ICommand RCRWPathCommand { get; }
        /// <summary>Указание расположения reactor-crw</summary>
        private void OnRCRWPathCommandExecuted(object parameter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = _defExt,
                Filter = _dlgFilter
            };

            if (dlg.ShowDialog() is true)
                RCRWPath = dlg.FileName;
        }

        private bool CanRCRWPathCommandExecute(object parameter) => true;

        #endregion

        #region Указание расположения загрузки контента
        /// <summary>Указание расположения загрузки контента</summary>
        public ICommand OutputPathCommand { get; }


        /// <summary>Указание расположения загрузки контента</summary>
        private void OnOutputPathCommandExecuted(object parameter)
        {
            using var dialog = new FolderBrowserDialog()
            {
                Description = _chooseDestinationFolderMessage,
                ShowNewFolderButton = true,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };

            if (dialog.ShowDialog() is DialogResult.OK)
                OutputPath = dialog.SelectedPath;
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

                    ImageCheckBox = config.ImageTypes[_image];
                    WebmCheckBox = config.ImageTypes[_webm];
                    GifCheckBox = config.ImageTypes[_gif];
                    Mp4CheckBox = config.ImageTypes[_mp4];
                }
            }
            catch (Exception ex)
            {
                Status = _unexpectedError + ex.InnerException.Message;
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
                ImageTypes = new(_ImageTypes),
            };

            try
            {
                if (_ConfigService.SaveConfig(config)) 
                    Status = _configSaved;
                else 
                    Status = _somethingWrong;
            }
            catch (Exception ex)
            {
                Status = _unexpectedError + ex.InnerException.Message;
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
        private const string _configSaved = "Настройки успешно сохранены.";
        private const string _somethingWrong = "Что-то пошло не так.";
        private const string _unexpectedError = "Непредвиденная ошибка!\n";
        private const string _image = "image";
        private const string _webm = "webm";
        private const string _gif = "gif";
        private const string _mp4 = "mp4";
        private const string _recator_crwUI = "_recator_crwUI";
        
        #region Константы для StringBuilder
        private const char _quotation = '"';
        private const char _comma = ',';
        private const string _p = " -p ";
        private const string _s = " -s ";
        private const string _w = " -w ";
        private const string _d = " -d ";
        private const string _c = " -c ";
        private const string _o = " -o ";

        #endregion

        #endregion

        #region Сервисы
        private readonly IConfigService _ConfigService;
        private readonly ILogger _log = Log.ForContext<MainWindowViewModel>();
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
                imgTypes.Append(_comma);
            }
            imgTypes.Remove(imgTypes.Length - 1, 1);

            str.Append(RCRWPath)
               .Append(_p)
               .Append(_quotation)
               .Append(URL)
               .Append(_quotation)
               .Append(_s)
               .Append(_quotation)
               .Append(imgTypes)
               .Append(_quotation)
               .Append(_w)
               .Append(NumOfWorkers);

            if (!string.IsNullOrEmpty(OutputPath))
                str.Append(_d)
                   .Append(_quotation)
                   .Append(OutputPath)
                   .Append(_quotation);

            if (CookiesAccepted && !string.IsNullOrEmpty(CookiesData))
                str.Append(_c)
                   .Append(_quotation)
                   .Append(CookiesData)
                   .Append(_quotation);

            if (OnePage)
                str.Append(_o);

            return str.ToString();
        }
        #endregion
    }
}

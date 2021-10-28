using System.Windows.Forms;
using reactor_crwUI.Core;

using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace reactor_crwUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelCore
    {
        public MainWindowViewModel()
        {
            #region Команды
            RCRWPathCommand = new LambdaCommand(OnRCRWPathCommandExecuted, CanRCRWPathCommandExecute);
            OutputPathCommand = new LambdaCommand(OnOutputPathCommandExecuted, CanOutputPathCommandExecute);
            StartCrawlCommand = new LambdaCommand(OnStartCrawlCommandExecuted, CanStartCrawlCommandExecute);

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
        private readonly Dictionary<string, bool> _ImageTypes = new()
        {
            {"image", true},
            { "gif", true },
            { "webm", true },
            { "mp4", true }
        };

        #endregion

        #region ImageCheckBox : bool - Выбирать изображения

        /// <summary>Выбирать изображения</summary>
        private bool _ImageCheckBox = true;

        /// <summary>Выбирать изображения</summary>
        public bool ImageCheckBox
        {
            get => _ImageCheckBox;
            set
            {
                if (Set(ref _ImageCheckBox, value))
                    _ImageTypes["image"] = value;
            }
        }
        #endregion

        #region GifCheckBox : bool - Выбрать гифки

        /// <summary>Выбрать гифки</summary>
        private bool _GifCheckBox = true;

        /// <summary>Выбрать гифки</summary>
        public bool GifCheckBox
        {
            get => _GifCheckBox;
            set
            {
                if (Set(ref _GifCheckBox, value))
                    _ImageTypes["gif"] = value;
            }
        }

        #endregion

        #region WebmCheckBox : bool - Выбрать webm

        /// <summary>Выбрать webm</summary>
        private bool _WebmCheckBox = true;

        /// <summary>Выбрать webm</summary>
        public bool WebmCheckBox
        {
            get => _WebmCheckBox;
            set
            {
                if (Set(ref _WebmCheckBox, value))
                    _ImageTypes["webm"] = value;
            }
        }

        #endregion

        #region Mp4CheckBox : bool - Выбрать mp4

        /// <summary>Выбрать mp4</summary>
        private bool _Mp4CheckBox = true;

        /// <summary>Выбрать mp4</summary>
        public bool Mp4CheckBox
        {
            get => _Mp4CheckBox;
            set
            {
                if (Set(ref _Mp4CheckBox, value))
                    _ImageTypes["mp4"] = value;

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
            
            var imgTypesSelected = _ImageTypes.Where(t => t.Value).Select(t => t.Value).FirstOrDefault();
            if (!imgTypesSelected)
                Status += _noItemsSelected;

            if (!CorrectUrl)
                Status += _uriErrorMessage;

            if (CorrectUrl && imgTypesSelected)
            {
                var args = BuildArgForCrawler();
                Status += "\n" + args;
            // TODO: Дописать запуск краулера после получения библиотеки
            }
        }
        private bool CanStartCrawlCommandExecute(object parameter) => true;

        #endregion

        #endregion

        #region Константы
        private const string _defExt = ".exe";
        private const string _dlgFilter = "Исполняемые файлы (.exe)|*.exe";
        private const string _uriErrorMessage = "\nОшибка в адресе сайта.";
        private const string _chooseDestinationFolderMessage = "Выберите папку, в которую будет загружен контент";
        private const string _noItemsSelected = "\nНе выбрано ни одного типа контента.";

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
            foreach (var item in _ImageTypes)
            {
                if (item.Value)
                {
                    imgTypes.Append(item.Key);
                    imgTypes.Append(",");
                }
            }
            imgTypes.Remove(imgTypes.Length - 1, 1);

            str = str
                .Append("-p ")
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

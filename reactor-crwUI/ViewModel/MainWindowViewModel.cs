using System.Windows.Forms;
using reactor_crwUI.Core;

using System.Windows.Input;

namespace reactor_crwUI.ViewModel
{
    class MainWindowViewModel : ViewModelCore
    {
        public MainWindowViewModel()
        {
            #region Команды
            RCRWPathCommand = new LambdaCommand(OnRCRWPathCommandExecuted, CanRCRWPathCommandExecute);
            OutputPathCommand = new LambdaCommand(OnOutputPathCommandExecuted, CanOutputPathCommandExecute);

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
        #endregion

        #region Команды

        #region Указание расположения reactor-crw
        /// <summary>Указание расположения reactor-crw</summary>
        public ICommand RCRWPathCommand { get; }
        /// <summary>Указание расположения reactor-crw</summary>
        private void OnRCRWPathCommandExecuted(object parameter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = defExt;
            dlg.Filter = dlgFilter;
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
                dialog.Description = "Выберите папку, в которую будет загружен контент";
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

        #endregion

        #region Константы
        private const string defExt = ".exe";
        private const string dlgFilter = "Исполняемые файлы (.exe)|*.exe";
        #endregion
    }
}

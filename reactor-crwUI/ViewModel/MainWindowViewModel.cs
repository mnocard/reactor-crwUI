using Microsoft.Win32;

using reactor_crwUI.Core;

using System;
using System.IO;
using System.Windows.Input;

namespace reactor_crwUI.ViewModel
{
    class MainWindowViewModel : ViewModelCore
    {
        public MainWindowViewModel()
        {
            #region Команды
            RCRWPathCommand = new LambdaCommand(OnRCRWPathCommandExecuted, CanRCRWPathCommandExecute);

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

        #endregion

        #region Команды

        #region Указание расположения reactor-crw
        /// <summary>Указание расположения reactor-crw</summary>
        public ICommand RCRWPathCommand { get; }
        /// <summary>Указание расположения reactor-crw</summary>
        private void OnRCRWPathCommandExecuted(object parameter)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = defExt;
            dlg.Filter = dlgFilter;
            var result = dlg.ShowDialog();

            if (result == true)
            {
                Title = dlg.FileName;
            }
        }

        private bool CanRCRWPathCommandExecute(object parameter) => true;

        #endregion
        #endregion

        #region Константы
        private const string defExt = ".exe";
        private const string dlgFilter = "Исполняемые файлы (.exe)|*.exe";
        #endregion
    }
}

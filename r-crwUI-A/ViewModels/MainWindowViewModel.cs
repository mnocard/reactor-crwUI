using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using r_crwUI_A.Core;
using r_crwUI_A.Interfaces;
using r_crwUI_A.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using r_crwUI_A.Views;
using System.ComponentModel;

namespace r_crwUI_A.ViewModels
{
    class MainWindowViewModel : ViewModelCore
    {
        #region Сервисы
        private readonly IConfigureProvider _ConfigureProvider;
        private readonly ILogger _Logger;
        #endregion


        public MainWindowViewModel(IConfigureProvider configureProvider, ILogger logger)
        {
            _Logger = logger;
            _ConfigureProvider = configureProvider;
            PropertyTransmitter.OnMessageTransmitted += BuildArg;
            PropertyTransmitter.OnDeleteEntity += DeleteProperty;

            _Logger.WriteLog("INFO: Создание MainWindowViewModel");
        }

        #region Свойста

        #region UCPropertyList : ObservableCollection<UserControl> - Список атрибутов для добавления в передаваемую строку
        /// <summary>Список атрибутов для добавления в передаваемую строку</summary>
        private ObservableCollection<UserControl> _UCPropertyList = new();

        /// <summary>Список атрибутов для добавления в передаваемую строку</summary>
        public ObservableCollection<UserControl> UCPropertyList
        {
            get => _UCPropertyList;
            set => Set(ref _UCPropertyList, value);
        }
        #endregion

        #region ResultString : string - Строка для передачи в командную строку

        /// <summary>Строка для передачи в командную строку</summary>
        private string _ResultString;

        /// <summary>Строка для передачи в командную строку</summary>
        public string ResultString
        {
            get => _ResultString;
            set => Set(ref _ResultString, value);
        }

        #endregion

        #region Status : string - Статусная строка

        /// <summary>Статусная строка</summary>
        private string _Status = StatusHelloWorld;

        /// <summary>Статусная строка</summary>
        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        #endregion

        #region ExeFilePath : string - Путь к исполняемому файлу
        /// <summary>Путь к исполняемому файлу</summary>
        private string _ExeFilePath;

        #endregion

        #region VMDataContextList : List<PropertyControlViewModel> - Список датаконтекстов для юзерконтролов
        /// <summary>Список датаконтекстов для юзерконтролов</summary>
        private List<PropertyControlViewModel> VMDataContextList = new();
        #endregion

        #endregion

        #region Команды
        /// <summary>
        /// Загрузка версии обновления со значениями из json файла
        /// </summary>
        /// <returns></returns>
        public async Task LoadConfig()
        {
            try
            {
                _Logger.WriteLog("INFO");
                var dialog = new OpenFileDialog
                {
                    AllowMultiple = false,
                    Title = _chooseDestinationFolderForLoad
                };
                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { _jsonExt },
                    Name = _dlgJsonFilter,
                });

                var paths = await dialog.ShowAsync(new Window());
                if (paths is not null &&
                    paths.Any())
                {
                    var path = paths.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        var dictionary = _ConfigureProvider.LoadDataFromJson<List<PropertyControlViewModel>>(path);
                        if (dictionary.Any())
                        {
                            _Logger.WriteLog("INFO");
                            VMDataContextList.Clear();
                            VMDataContextList.AddRange(dictionary);
                            UpdateAttributesList();
                            Status = string.Format(StatusSuccessfulUpload, Path.GetFileName(path));
                            _Logger.WriteLog("DONE");
                        }
                    }
                }
                _Logger.WriteLog("DONE");
            }
            catch (Exception e)
            {
                _Logger.WriteLog("FATAL\n" + e.Message);
                Status = StatusUnexpectedErrorInLoadProcess;
            }
        }

        /// <summary>
        /// Сохранение версии обновления со значениями в json файл
        /// </summary>
        /// <returns></returns>
        public async Task SaveConfig()
        {
            try
            {
                _Logger.WriteLog("INFO");

                if (!UCPropertyList.Any() ||
                    !VMDataContextList.Any())
                {
                    Status = StatusNothingToSave;
                    return;
                }

                var dialog = new SaveFileDialog
                {
                    Title = _chooseDestinationFolderForSave
                };

                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { _jsonExt },
                    Name = _dlgJsonFilter,
                });

                var result = await dialog.ShowAsync(new Window());
                if (!string.IsNullOrEmpty(result))
                {
                    Status = _ConfigureProvider.SaveDataToJson(VMDataContextList, result) ?
                        StatusSuccessfulSaved :
                        StatusUnexpectedErrorInSaveProcess;
                }
                _Logger.WriteLog("DONE");
            }
            catch (Exception e)
            {
                _Logger.WriteLog("FATAL\n" + e.Message);
                Status = StatusUnexpectedErrorInSaveProcess;
            }
        }

        /// <summary>
        /// Указание пути до исполняемого файла
        /// </summary>
        /// <returns></returns>
        public async Task SelectPath()
        {
            try
            {
                _Logger.WriteLog("INFO");

                var dialog = new OpenFileDialog
                {
                    AllowMultiple = false,
                    Title = _chooseDestinationFolderForLoad,
                };

                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { _exeExt },
                    Name = _dlgExeFilter,
                });

                var paths = await dialog.ShowAsync(new Window());
                if (paths.Any())
                {
                    var path = paths.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        _ExeFilePath = path;
                        BuildArg();
                        _Logger.WriteLog("DONE");
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.WriteLog("FATAL\n" + e.Message);
                Status = StatusUnexpectedErrorExeFile;
            }
        }

        /// <summary>
        /// Отображение окна-справки
        /// </summary>
        public void ShowHelp()
        {
            _Logger.WriteLog("INFO");

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new HelpWindow
                {
                    Height = 130,
                    Width = 300,
                    CanResize = false
                };
                window.ShowDialog(desktop.MainWindow);
                _Logger.WriteLog("DONE");
            }
        }

        /// <summary>
        /// Запуск командной строки с указаными аргументами
        /// </summary>
        public void StartProcess()
        {
            _Logger.WriteLog("INFO");

            if (string.IsNullOrEmpty(ResultString))
                Status = StatusWhatAreYouDoingHere;
            else if (!File.Exists(_ExeFilePath))
                Status = StatusExeFileNotFound;
            else if (!VMDataContextList.Any(x => !string.IsNullOrEmpty(x.ValueProperty)))
                Status = StatusKeyValuePairsNotFound;
            else
            {
                Status = StatusStartUpdate;
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(_ExeFilePath),
                    FileName = _cmd,
                    Arguments = ArgStart + ResultString,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                });
                _Logger.WriteLog("DONE");
            }
        }

        /// <summary>
        /// Добавление новой строки ключ-значение
        /// </summary>
        public void AddAttribute()
        {
            _Logger.WriteLog("INFO");

            var dataContext = new PropertyControlViewModel();
            VMDataContextList.Add(dataContext);
            UCPropertyList.Add(new PropertyControl
            {
                DataContext = dataContext
            });

            _Logger.WriteLog("DONE");
        }

        /// <summary>
        /// Закрытие программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosingWindow(object sender, CancelEventArgs e)
        {
            _Logger.WriteLog("EXIT");
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Обновление данных
        /// </summary>
        private void UpdateAttributesList()
        {
            _Logger.WriteLog("INFO");
         
            UCPropertyList.Clear();

            foreach (var item in VMDataContextList)
            {
                var control = new PropertyControl { DataContext = item };
                UCPropertyList.Add(control);
            }
            _Logger.WriteLog("DONE");
        }

        /// <summary>
        /// Создание строки аргументов для передачи в командную строку
        /// </summary>
        private void BuildArg()
        {
            var str = new StringBuilder();

            if (File.Exists(_ExeFilePath))
                str.Append(Path.GetFileName(_ExeFilePath))
                    .Append(ArgWhiteSpace);

            if (VMDataContextList.Any())
                foreach (var item in VMDataContextList)
                {
                    str.Append(item.KeyProperty)
                        .Append(ArgWhiteSpace);

                    if (!string.IsNullOrEmpty(item.ValueProperty))
                        str.Append(ArgQuotation)
                            .Append(item.ValueProperty)
                            .Append(ArgQuotation)
                            .Append(ArgWhiteSpace);
                }

            ResultString = str.ToString();
        }

        /// <summary>
        /// Удаление контрола со строкой ключ-значение
        /// </summary>
        /// <param name="property">Удаляемый контрол</param>
        private void DeleteProperty(PropertyControlViewModel property)
        {
            _Logger.WriteLog("INFO");

            var control = UCPropertyList.FirstOrDefault(x => Equals(x.DataContext, property));
            if (control != null)
            {
                VMDataContextList.Remove(property);
                UpdateAttributesList();
            }

            _Logger.WriteLog("DONE");
        }

        #endregion

        #region Константы

        private const string _chooseDestinationFolderForLoad = "Выберите файл для загрузки.";
        private const string _chooseDestinationFolderForSave = "Выберите папку, в которую будет сохранён конфигурационный файл.";
        private const string _jsonExt = "json";
        private const string _exeExt = "exe";
        private const string _dlgJsonFilter = "Json documents (.json)|*.json";
        private const string _dlgExeFilter = "Executable files (.exe)|*.exe";

        private const string StatusHelloWorld = "Привет!";
        private const string StatusSuccessfulUpload = "Конфигурационный файл \"{0}\" успешно загружен";
        private const string StatusUnexpectedErrorInLoadProcess = "Непредвиденная ошибка при попытке загрузки файла!";
        private const string StatusNothingToSave = "Нечего сохранять.";
        private const string StatusSuccessfulSaved = "Данные успешно сохранены.";
        private const string StatusUnexpectedErrorInSaveProcess = "Непредвиденная ошибка при сохранении данных!";
        private const string StatusUnexpectedErrorExeFile = "Непредвиденная ошибка при указании пути к исполняемому файлу!";
        private const string StatusExeFileNotFound = "Укажите путь к исполняемому файлу.";
        private const string StatusKeyValuePairsNotFound = "Не указано ни одной пары ключ-значение";
        private const string StatusWhatAreYouDoingHere = "Надо указать путь к исполняемому файлу, а потом выбрать конфигурационный файл и ввести значения ключам.";
        private const string StatusStartUpdate = "Поехали!";

        private const string _cmd = "cmd";
        private const string ArgStart = "/k START ";
        private const char ArgWhiteSpace = ' ';
        private const char ArgQuotation = '"';

        #endregion
    }
}
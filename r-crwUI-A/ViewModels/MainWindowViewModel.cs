using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using r_crwUI_A.Core;
using r_crwUI_A.Interfaces;
using r_crwUI_A.Services;
using r_crwUI_A.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using r_crwUI_A.Model;

namespace r_crwUI_A.ViewModels
{
    class MainWindowViewModel : ViewModelCore
    {
        #region Сервисы
        private readonly ILoadDefaultSettings _loadDefaultSettings;
        private readonly IConfigureProvider _ConfigureProvider;
        private readonly ILogger _Logger;
        #endregion

        public MainWindowViewModel(ILoadDefaultSettings loadDefaultSettings, IConfigureProvider configureProvider, ILogger logger)
        {
            _Logger = logger;
            _loadDefaultSettings = loadDefaultSettings;
            _ConfigureProvider = configureProvider;
            PropertyTransmitter.OnMessageTransmitted += BuildArg;
            PropertyTransmitter.OnDeleteEntity += DeleteProperty;

            _Logger.WriteLog(LogInfo + LogCreateVM);
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

        #region Separator : string - Разделитель между ключом и значением

        /// <summary>Разделитель между ключом и значением</summary>
        private string _Separator = ArgWhiteSpace.ToString();

        /// <summary>Разделитель между ключом и значением</summary>
        public string Separator
        {
            get => _Separator;
            set
            {
                Set(ref _Separator, value);
                BuildArg();
            }
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

        #region Options : Dictionary<string, string> - Список датаконтекстов для юзерконтролов
        /// <summary>Список датаконтекстов для юзерконтролов</summary>
        private readonly Dictionary<string, string> Options = new Dictionary<string, string>
        {
            { _separatorOption, "" },
            { _exePathOption, "" }
        };
        #endregion

        #endregion

        #region Команды
        /// <summary>
        /// Загрузка атрибутов со значениями из json файла
        /// </summary>
        /// <returns></returns>
        public async Task LoadConfig()
        {
            try
            {
                _Logger.WriteLog(LogInfo);
                var dialog = new OpenFileDialog
                {
                    AllowMultiple = false,
                    Title = ChooseDestinationFolderForLoad,
                };
                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { JsonExt },
                    Name = DlgJsonFilter,
                });

                var paths = await dialog.ShowAsync(new Window());
                if (paths is not null &&
                    paths.Any())
                {
                    FillDictionary(paths.FirstOrDefault());
                }
                _Logger.WriteLog(LogDone);
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Сохранение атрибутов со значениями в json файл
        /// </summary>
        /// <returns></returns>
        public async Task SaveConfig()
        {
            try
            {
                _Logger.WriteLog(LogInfo);

                if (!UCPropertyList.Any() ||
                    !VMDataContextList.Any())
                {
                    Status = StatusNothingToSave;
                    return;
                }

                var dialog = new SaveFileDialog
                {
                    Title = ChooseDestinationFolderForSave,
                    InitialFileName = DlgDefaultFileName + DateTime.Now.ToString(DateTimeFormat),
                };

                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { JsonExt },
                    Name = DlgJsonFilter,
                });

                var result = await dialog.ShowAsync(new Window());
                if (!string.IsNullOrEmpty(result))
                {
                    var settings = new Settings
                    {
                        _Dictionary = Options,
                        _PropertyControl = VMDataContextList,
                    };
                    Status = _ConfigureProvider.SaveDataToJson(settings, result) ?
                        StatusSuccessfulSaved :
                        StatusUnexpectedError;
                }
                _Logger.WriteLog(LogDone);
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
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
                _Logger.WriteLog(LogInfo);

                var dialog = new OpenFileDialog
                {
                    AllowMultiple = false,
                    Title = ChooseDestinationFolderForLoad,
                };

                dialog.Filters.Add(new FileDialogFilter
                {
                    Extensions = new List<string> { ExeExt },
                    Name = DlgExeFilter,
                });

                var paths = await dialog.ShowAsync(new Window());
                if (paths is not null &&
                    paths.Any())
                {
                    var path = paths.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        _ExeFilePath = path;
                        BuildArg();
                        _Logger.WriteLog(LogDone);
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
            }
        }

        /// <summary>
        /// Отображение окна-справки
        /// </summary>
        public void ShowHelp()
        {
            try
            {
                _Logger.WriteLog(LogInfo);

                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var dataContext = new HelpWindowViewModel
                    {
                        Title = HelpTitle,
                        Message = Email,
                    };

                    var window = new HelpWindow
                    {
                        DataContext = dataContext,
                        Height = 130,
                        Width = 300,
                        CanResize = false
                    };
                    window.ShowDialog(desktop.MainWindow);
                    _Logger.WriteLog(LogDone);
                }
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
            }
        }

        /// <summary>
        /// Запуск командной строки с указаными аргументами
        /// </summary>
        public void StartProcess()
        {
            try
            {
                _Logger.WriteLog(LogInfo);

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
                        FileName = Cmd,
                        Arguments = ArgStart + ResultString,
                        WindowStyle = ProcessWindowStyle.Normal,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                    });
                    _Logger.WriteLog(LogDone);
                }
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Добавление новой строки ключ-значение
        /// </summary>
        public void AddAttribute()
        {
            try
            {
                _Logger.WriteLog(LogInfo);

                var dataContext = new PropertyControlViewModel();
                VMDataContextList.Add(dataContext);
                UCPropertyList.Add(new PropertyControl
                {
                    DataContext = dataContext
                });

                _Logger.WriteLog(LogDone);
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Закрытие программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosingWindow(object? sender, CancelEventArgs e)
        {
            _Logger.WriteLog(LogExit);
        }

        /// <summary>
        /// Открытие программы
        /// </summary>
        public void OnOpenedWindow(object? sender, EventArgs e)
        {
            try
            {
                _Logger.WriteLog("Попытка найти и загрузить файлы настроек и исполняемый файл из корневой папки.");
                var settingsFileName = _loadDefaultSettings.GetDefaultSettings();
                FillDictionary(settingsFileName);

                if (string.IsNullOrEmpty(_ExeFilePath))
                    _ExeFilePath = _loadDefaultSettings.GetDefaultFilePath();
                BuildArg();
            }
            catch (Exception ex)
            {
                _Logger.WriteLog(LogFatal + ex.Message);
            }
        }

        #endregion

        #region Приватные методы

        private void FillDictionary(string settingsFileName)
        {
            if (!string.IsNullOrWhiteSpace(settingsFileName))
            {
                _Logger.WriteLog(LogInfo);
                var settings = _ConfigureProvider.LoadDataFromJson<Settings>(settingsFileName);

                Separator = settings._Dictionary[_separatorOption];
                _ExeFilePath = settings._Dictionary[_exePathOption];

                if (settings._PropertyControl != null)
                {
                    VMDataContextList.Clear();
                    VMDataContextList.AddRange(settings._PropertyControl);
                }
                UpdateAttributesList();
                Status = string.Format(StatusSuccessfulUpload, Path.GetFileName(settingsFileName));
                _Logger.WriteLog(LogDone);
            }
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        private void UpdateAttributesList()
        {
            try
            {
                _Logger.WriteLog(LogInfo);

                UCPropertyList.Clear();

                foreach (var item in VMDataContextList)
                {
                    var control = new PropertyControl { DataContext = item };
                    UCPropertyList.Add(control);
                }
                _Logger.WriteLog(LogDone);
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Создание строки аргументов для передачи в командную строку
        /// </summary>
        private void BuildArg()
        {
            try
            {
                var str = new StringBuilder();

                if (File.Exists(_ExeFilePath))
                    str.Append(Path.GetFileName(_ExeFilePath))
                        .Append(ArgWhiteSpace);

                if (VMDataContextList.Any())
                    foreach (var item in VMDataContextList)
                    {
                        str.Append(item.KeyProperty);

                        if (!string.IsNullOrEmpty(item.ValueProperty))
                            str.Append(Separator)
                                .Append(ArgQuotation)
                                .Append(item.ValueProperty)
                                .Append(ArgQuotation);

                        str.Append(ArgWhiteSpace);
                    }

                ResultString = str.ToString();
                RefreshOptions();
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Удаление контрола со строкой ключ-значение
        /// </summary>
        /// <param name="property">Удаляемый контрол</param>
        private void DeleteProperty(PropertyControlViewModel property)
        {
            try
            {
                _Logger.WriteLog(LogInfo);

                var control = UCPropertyList.FirstOrDefault(x => Equals(x.DataContext, property));
                if (control != null)
                {
                    VMDataContextList.Remove(property);
                    UpdateAttributesList();
                }

                _Logger.WriteLog(LogDone);
            }
            catch (Exception e)
            {
                _Logger.WriteLog(LogFatal + e);
                Status = StatusUnexpectedError;
                ShowExceptionMessage(e.Message);
            }
        }

        /// <summary>
        /// Отображение сообщения об ошибке
        /// </summary>
        /// <param name="exception">Сообщение об ошибке</param>
        private void ShowExceptionMessage(string exception)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dataContext = new HelpWindowViewModel
                {
                    Title = StatusUnexpectedError,
                    Message = string.Format(LookForLogs, exception),
                };

                var window = new HelpWindow
                {
                    DataContext = dataContext,
                    Height = 130,
                    Width = 300,
                    CanResize = false
                };
                window.ShowDialog(desktop.MainWindow);
            }
        }

        private void RefreshOptions()
        {
            if (Options.ContainsKey(_separatorOption))
                Options[_separatorOption] = Separator;
            else
                Options.Add(_separatorOption, Separator);

            if (Options.ContainsKey(_exePathOption))
                Options[_exePathOption] = _ExeFilePath;
            else
                Options.Add(_exePathOption, _ExeFilePath);
        }
        #endregion

        #region Константы

        private const string ChooseDestinationFolderForLoad = "Выберите файл для загрузки.";
        private const string ChooseDestinationFolderForSave = "Выберите папку, в которую будет сохранён конфигурационный файл.";
        private const string JsonExt = "json";
        private const string ExeExt = "exe";
        private const string DlgJsonFilter = "Json documents (.json)|*.json";
        private const string DlgExeFilter = "Executable files (.exe)|*.exe";
        private const string DlgDefaultFileName = "settings ";

        private const string StatusHelloWorld = "Привет!";
        private const string StatusSuccessfulUpload = "Конфигурационный файл \"{0}\" успешно загружен";
        private const string StatusNothingToSave = "Нечего сохранять.";
        private const string StatusSuccessfulSaved = "Данные успешно сохранены.";
        private const string StatusUnexpectedError = "Непредвиденная ошибка";
        private const string StatusExeFileNotFound = "Укажите путь к исполняемому файлу.";
        private const string StatusKeyValuePairsNotFound = "Не указано ни одной пары ключ-значение";
        private const string StatusWhatAreYouDoingHere = "Надо указать путь к исполняемому файлу, а потом выбрать конфигурационный файл и ввести значения ключам.";
        private const string StatusStartUpdate = "Поехали!";
        private const string LookForLogs = "{0}\nПодробности в логах.";
        private const string DateTimeFormat = "yyyy.MM.dd HH.mm.ss";
        private const string Cmd = "cmd";
        private const string ArgStart = "/k START ";
        private const char ArgWhiteSpace = ' ';
        private const char ArgQuotation = '"';

        private const string LogInfo = "INFO";
        private const string LogDone = "DONE";
        private const string LogFatal = "FATAL\n";
        private const string LogExit = "EXIT";
        private const string LogCreateVM = "\nСоздание MainWindowViewModel";
        private const string HelpTitle = "Помощь";
        private const string Email = "mnocard@gmail.com";

        private const string _separatorOption = "Separator";
        private const string _exePathOption = "Path to executable file";
        #endregion
    }
}

namespace r_crwUI_A.Interfaces
{
    /// <summary>
    /// Сервис получения данных при первой загрузке приложения из корневой папки приложения
    /// </summary>
    internal interface ILoadDefaultSettings
    {
        /// <summary>
        /// Получить путь к последнему файлу настроек
        /// </summary>
        /// <returns>Путь к файлу</returns>
        string GetDefaultSettings();
        /// <summary>
        /// Получить путь к первому найденному в корневой папке приложения исполняемому файлу
        /// </summary>
        /// <returns>Путь к файлу</returns>
        string GetDefaultFilePath();
    }
}

namespace r_crwUI_A.Interfaces
{
    /// <summary>
    /// Сервис сохранения и загрузки файлов в json формате
    /// </summary>
    internal interface IConfigureProvider
    {
        /// <summary>
        /// Сохранение данных в json файл
        /// </summary>
        /// <typeparam name="T">Тип сохраняемых данных</typeparam>
        /// <param name="data">Данные для загрузки</param>
        /// <param name="filename">Путь к файлу</param>
        /// <returns>True если файл успешно сохраненен</returns>
        bool SaveDataToJson<T>(T data, string filename);
        /// <summary>
        /// Загрузка данных из json файла
        /// </summary>
        /// <typeparam name="T">Тип загружаемых данных</typeparam>
        /// <param name="filename">Путь к файлу</param>
        /// <returns>Загруженные из файла данные</returns>
        T LoadDataFromJson<T>(string filename);
    }
}

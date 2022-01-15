using r_crwUI_A.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace r_crwUI_A.Services
{
    internal class ConfigureProvider : IConfigureProvider
    {
        public bool SaveDataToJson<T>(T data, string filename)
        {
            try
            {
                if (data is null ||
                    (data is IEnumerable<object> list && !list.Any()) ||
                    string.IsNullOrEmpty(filename))
                    return false;

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                };

                var jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filename, jsonString);

                return File.Exists(filename);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно сохранить файл!", ex);
            }
        }

        public T LoadDataFromJson<T>(string filename)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var jsonString = File.ReadAllText(filename);
                    return JsonSerializer.Deserialize<T>(jsonString, options);
                }

                return default;
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно прочитать файл!", ex);
            }
        }
    }
}
using System.Collections.Generic;

namespace reactor_crwUI.Model
{
    public class Config
    {
        public string RCRWPath { get; set; }
        public bool CookiesAccepted { get; set; }
        public string CookiesData { get; set; }
        public string OutputPath { get; set; }
        public string URL { get; set; }
        public Dictionary<string, bool> ImageTypes { get; set; }
        public bool OnePage { get; set; }
        public int NumOfWorkers { get; set; }
    }
}

using r_crwUI_A.ViewModels;
using System.Collections.Generic;

namespace r_crwUI_A.Model
{
    internal record Settings
    {
        public Dictionary<string, string> _Dictionary { get; set; }
        public List<PropertyControlViewModel> _PropertyControl { get; set; }
    }
}

using r_crwUI_A.Core;
using r_crwUI_A.Services;

namespace r_crwUI_A.ViewModels
{
    class PropertyControlViewModel : ViewModelCore
    {
        public PropertyControlViewModel() { }
        public PropertyControlViewModel(string key, string value)
        {
            KeyProperty = key;
            ValueProperty = value;
        }

        private string _KeyProperty;
        public string KeyProperty
        {
            get => _KeyProperty;
            set
            {
                _KeyProperty = value;
                PropertyTransmitter.BroadCast();
            }
        }

        private string _ValueProperty;

        public string ValueProperty
        {
            get => _ValueProperty;
            set
            {
                _ValueProperty = value;
                PropertyTransmitter.BroadCast();
            }
        }

        public void DeleteControl()
        {
            PropertyTransmitter.DeleteProperty(this);
        }
    }
}

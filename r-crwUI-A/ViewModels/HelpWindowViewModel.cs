using r_crwUI_A.Core;

namespace r_crwUI_A.ViewModels
{
    internal class HelpWindowViewModel : ViewModelCore
    {
        public HelpWindowViewModel() { }

        #region Message : string - Сообщение окна

        /// <summary>Сообщение окна</summary>
        private string _Message;

        /// <summary>Сообщение окна</summary>
        public string Message
        {
            get => _Message;
            set => Set(ref _Message, value);
        }

        #endregion

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title;

        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion
    }
}

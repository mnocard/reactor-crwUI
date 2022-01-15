using r_crwUI_A.ViewModels;

using System;

namespace r_crwUI_A.Services
{
    internal static class PropertyTransmitter
    {
        public static void BroadCast()
        {
            if (OnMessageTransmitted != null)
                OnMessageTransmitted();
        }

        public static void DeleteProperty(PropertyControlViewModel property)
        {
            if (OnDeleteEntity != null)
                OnDeleteEntity(property);
        }

        public static Action OnMessageTransmitted;
        public static Action<PropertyControlViewModel> OnDeleteEntity;
    }
}

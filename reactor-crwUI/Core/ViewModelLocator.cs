using Microsoft.Extensions.DependencyInjection;

using reactor_crwUI.ViewModel;

namespace reactor_crwUI.Core
{
    class ViewModelLocator
    {
        public MainWindowViewModel MWModel => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}

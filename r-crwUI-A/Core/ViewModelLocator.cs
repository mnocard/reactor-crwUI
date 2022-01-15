using Microsoft.Extensions.DependencyInjection;

using r_crwUI_A.ViewModels;

namespace r_crwUI_A.Core
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MWModel => App.Services.GetRequiredService<MainWindowViewModel>();
        public HelpWindowViewModel HWModel => App.Services.GetRequiredService<HelpWindowViewModel>();
    }
}

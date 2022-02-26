using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using r_crwUI_A.ViewModels;

namespace r_crwUI_A
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var vm = App.Services.GetRequiredService<MainWindowViewModel>();
            Closing += vm.OnClosingWindow;
            Opened += vm.OnOpenedWindow;
        }

    }
}

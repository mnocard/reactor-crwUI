using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace r_crwUI_A.Views
{
    public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
}

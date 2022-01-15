using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace r_crwUI_A.Views
{
    public partial class PropertyControl : UserControl
{
    public PropertyControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
}

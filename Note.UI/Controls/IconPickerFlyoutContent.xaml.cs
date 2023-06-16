using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WAH.NoteSystem.Core;
using System;
using Get.XAMLTools;

namespace WAH.NoteSystem.UI.Controls;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[DependencyProperty<SymbolEx>("Icon")]
public sealed partial class IconPickerFlyoutControl
{
    public event Action<SymbolEx>? SelectionChanged;
    readonly static SymbolEx[] Values = Enum.GetValues<SymbolEx>();
    public IconPickerFlyoutControl()
    {
        this.InitializeComponent();
        GridView.ItemsSource = Values;
        GridView.SelectionChanged += Root_SelectionChanged;
    }

    private void Root_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionChanged?.Invoke((SymbolEx)GridView.SelectedItem);
    }
}

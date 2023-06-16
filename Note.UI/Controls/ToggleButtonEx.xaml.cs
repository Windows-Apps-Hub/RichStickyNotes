using Get.XAMLTools;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls.Primitives;
namespace WAH.NoteSystem.UI.Controls;
[DependencyProperty<FormatEffect>("FormatEffect", CheckForChanges = true, GenerateLocalOnPropertyChangedMethod = true)]
public sealed partial class ToggleButtonEx : ToggleButton
{
    public ToggleButtonEx()
    {
        InitializeComponent();
    }

    private void ToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        FormatEffect = FormatEffect.Toggle;
    }
    partial void OnFormatEffectChanged(FormatEffect oldValue, FormatEffect newValue)
    {
        if (oldValue == newValue) return;
        switch (newValue)
        {
            case FormatEffect.Toggle:
                break;
            case FormatEffect.On:
                IsChecked = true;
                break;
            case FormatEffect.Off:
                IsChecked = false;
                break;
            case FormatEffect.Undefined:
                IsChecked = false;
                break;
        }
    }
}

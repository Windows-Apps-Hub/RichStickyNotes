using Get.XAMLTools;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

namespace WAH.NoteSystem.UI.Controls;
[DependencyProperty<Color?>("ColorNullable", GenerateLocalOnPropertyChangedMethod = true)]
partial class ColorPickerEx : ColorPicker
{
    public ColorPickerEx()
    {
        ColorChanged += delegate
        {
            if (Color != ColorNullable)
            {
                ColorNullable = Color;
            }
        };
    }
    partial void OnColorNullableChanged(Color? oldValue, Color? newValue)
    {
        if (newValue is not null && newValue != Color)
        {
            Color = newValue.Value;
        }
    }
}

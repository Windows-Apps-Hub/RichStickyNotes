using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WAH.NoteSystem.UI.Controls;

public class CustomFlyout : FlyoutBase
{
    Control Control;
    public CustomFlyout(Control control)
    {
        Control = control;
    }

    protected override Control CreatePresenter()
    {
        return Control;
    }
}

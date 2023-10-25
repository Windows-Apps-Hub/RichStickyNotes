using Microsoft.UI.Xaml;
using WAH.NoteSystem.Core.Tags;
using Get.XAMLTools;

namespace WAH.NoteSystem.UI.Controls;

[DependencyProperty<Tag>("NoteTag", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public sealed partial class TagPresenter
{

    public TagPresenter()
    {
        InitializeComponent();
    }
}

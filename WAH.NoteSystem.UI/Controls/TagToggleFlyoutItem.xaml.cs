using WAH.NoteSystem.UI.Helpers;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Get.XAMLTools;
namespace WAH.NoteSystem.UI.Controls;
[DependencyProperty<Note>("Note", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
[DependencyProperty<Tag>("NoteTag", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public sealed partial class TagToggleFlyoutItem
{
    public TagToggleFlyoutItem()
    {
        InitializeComponent();
    }
    bool ContainsTag(Note note, Tag tag) => note.Tags.Contains(tag);
    async void SetContainsTag(bool newValue)
    {
        var Note = this.Note;
        var Tag = NoteTag;
        if (Note is null || Tag is null) return;
        if (Note.Tags.Contains(Tag) == newValue) return;
        if (newValue)
        {
            await Note.RemoveTagAsync(Tag);
        } else
        {
            await Note.AddTagAsync(Tag);
        }
    }
}
class TagMenuFlyoutObseravableCollectionMimicker : ObservableCollectionMimicker<Tag, MenuFlyoutItemBase>
{
    public TagMenuFlyoutObseravableCollectionMimicker(
        ObservableCollection<Tag> source, IList<MenuFlyoutItemBase> dest
    ) : base(source, dest) { }
    protected override MenuFlyoutItemBase CreateFrom(Tag source)
    {
        return new TagToggleFlyoutItem { NoteTag = source };
    }
}

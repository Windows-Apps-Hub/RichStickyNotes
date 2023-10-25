using WAH.NoteSystem.UI.Helpers;
using WAH.NoteSystem.Core;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Get.XAMLTools;
namespace WAH.NoteSystem.UI.Controls;
[DependencyProperty<Note>("Note", GenerateLocalOnPropertyChangedMethod = true, UseNullableReferenceType = true)]
public sealed partial class TagEditorFlyoutContent : StackPanel, INotifyPropertyChanged
{
    public TagEditorFlyoutContent()
    {
        InitializeComponent();
    }
    partial void OnNoteChanged(Note? OldValue, Note? NewValue)
    {
        mimicker?.Dispose();
        if (NewValue is not null)
        {
            mimicker = new(NewValue.NoteSystem.Tags, NoteTagOC, NewValue);
            mimicker.ResetAndReadd();
        }
    }

    private async void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (Note is null) return;
        var f = (CheckBox)sender;
        var t = (Tag)f.Tag;
        await Note.AddTagAsync(t);
    }
    private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (Note is null) return;
        var f = (CheckBox)sender;
        var t = (Tag)f.Tag;
        await Note.RemoveTagAsync(t);
    }
    IEnumerable<Tag> Difference(ReadOnlyObservableCollection<Tag> t1, ReadOnlyObservableCollection<Tag> t2)
        => from x in t1
           where !t2.Contains(x)
           select x;
    ObservableCollection<NoteTag> NoteTagOC = new();
    Mimicker? mimicker;

    public event PropertyChangedEventHandler? PropertyChanged;
    SymbolEx _NewTagIcon = SymbolEx.Tag;
    void IconSelectionChanged(SymbolEx newVal)
    {
        if (_NewTagIcon == newVal) return;
        _NewTagIcon = newVal;
        TagIconFlyout.Hide();
        NewTagSymbolIcon.SymbolEx = newVal;
    }

    class Mimicker : ObservableCollectionMimicker<ReadOnlyObservableCollection<Tag>, Tag, NoteTag>
    {
        readonly Note Note;
        public Mimicker(ReadOnlyObservableCollection<Tag> source, IList<NoteTag> dest, Note note) : base(source, dest)
        {
            Note = note;
        }

        protected override NoteTag CreateFrom(Tag source)
        {
            return new(Note, source);
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (Note is null) return;
        var tag = await Note.NoteSystem.RegsiterTagAsync(NewTagTB.Text);
        tag.Icon = _NewTagIcon;
        await Note.AddTagAsync(tag);
        NewTagTB.Text = "";
    }
}

class NoteTag : INotifyPropertyChanged
{
    public Note Note { get; }
    public Tag Tag { get; }
    public NoteTag(Note Note, Tag Tag) {
        this.Note = Note;
        this.Tag = Tag;
        ((INotifyCollectionChanged)Note.Tags).CollectionChanged += (_, _) =>
            NotifyPropertyChanged(nameof(HasTag));
    }
    public bool HasTag
    {
        get => Note.Tags.Contains(Tag);
        set
        {
            if (value != HasTag)
            {
                var dq = DispatcherQueue.GetForCurrentThread();
                Task a;
                if (value)
                {
                    a = Note.AddTagAsync(Tag);
                } else
                {
                    a = Note.RemoveTagAsync(Tag);
                }
                Task.Run(async delegate
                {
                    await a;
                    dq.TryEnqueue(delegate
                    {
                        NotifyPropertyChanged(nameof(HasTag));
                    });
                });
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new(PropertyName));
    }
}
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using WAH.NoteSystem.Core.Notes;

namespace WAH.NoteSystem.Core.Tags;
[DebuggerDisplay("Tag {Name} with id {InternalId} and {Notes.Count} notes")]
public partial class Tag
{
    public bool IsValid { get; private set; } = true;

    public NoteSystem NoteSystem { get; }
    public ReadOnlyObservableCollection<Note> Notes { get; }

    public string Name
    {
        get => tagJSON.Name;
        set
        {
            tagJSON.Name = value;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                await tagJSON.SaveAsync(TagFile);
                dq.TryEnqueue(delegate
                {
                    NotifyPropertyChanged(nameof(Name));
                });
            });
        }
    }

    public partial Task AddNote(Note note);
    public partial Task RemoveNote(Note note);
    public partial void Delete();

    public Windows.UI.Color Color
    {
        get => tagJSON.Color;
        set
        {
            tagJSON.Color = value;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                await tagJSON.SaveAsync(TagFile);
                dq.TryEnqueue(delegate
                {
                    NotifyPropertyChanged(nameof(Color));
                });
            });
        }
    }

    public SymbolEx Icon
    {
        get => tagJSON.Icon;
        set
        {
            tagJSON.Icon = value;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                await tagJSON.SaveAsync(TagFile);
                dq.TryEnqueue(delegate
                {
                    NotifyPropertyChanged(nameof(Icon));
                });
            });
        }
    }
}
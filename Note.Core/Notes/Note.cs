using Microsoft.UI.Dispatching;
using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Tags;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;

namespace WAH.NoteSystem.Core.Notes;

[DebuggerDisplay("Note {Name} with id {InternalId} and {Tags.Count} tags")]
public partial class Note
{
    public bool IsValid { get; private set; } = true;
    public NoteSystem NoteSystem { get; }
    
    public ReadOnlyObservableCollection<Tag> Tags { get; }
    public ReadOnlyObservableCollection<NoteVersion> Versions { get; }
    public NoteVersion CurrentVersion => Versions[^1];

    public partial Task<NoteVersion> SaveNewVersionAsync(NoteData Data);
    public partial Task AddTagAsync(Tag tag);
    public partial Task RemoveTagAsync(Tag tag);
    public partial Task DeleteAsync();


    public string Name
    {
        get => noteMetadataJSON.Name;
        set
        {
            noteMetadataJSON.Name = value;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                await noteMetadataJSON.SaveAsync(noteMetadataFS);
                dq.TryEnqueue(delegate
                {
                    NotifyPropertyChanged(nameof(Name));
                });
            });
        }
    }
    public Color Color
    {
        get => noteMetadataJSON.Color;
        set
        {
            noteMetadataJSON.Color = value;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                await noteMetadataJSON.SaveAsync(noteMetadataFS);
                dq.TryEnqueue(delegate
                {
                    NotifyPropertyChanged(nameof(Color));
                });
            });
        }
    }
    public bool IsStarred
    {
        get => Tags.Contains(NoteSystem.StarTag);
        set
        {
            if (value != IsStarred)
            {
                var dq = DispatcherQueue.GetForCurrentThread();
                Task task;
                if (value)
                {
                    task = AddTagAsync(NoteSystem.StarTag);
                }
                else
                {
                    task = RemoveTagAsync(NoteSystem.StarTag);
                }

                Task.Run(async delegate
                {
                    await task;
                    dq.TryEnqueue(delegate
                    {
                        NotifyPropertyChanged(nameof(IsStarred));
                    });
                });
            }
        }
    }
}
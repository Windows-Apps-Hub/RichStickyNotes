using WAH.NoteSystem.Core.Storage;
using WAH.NoteSystem.Core.Tags;
using System.Threading.Tasks;
using System;

namespace WAH.NoteSystem.Core.Notes;

public partial class Note
{
    public partial async Task<NoteVersion> SaveNewVersionAsync(NoteData Data)
    {
        if (!IsValid) throw new InvalidOperationException();
        IFileStorage versionFS = await HistoryFolder.GetFileAsync($"{noteMetadataJSON.NextVersionIndex++}.NoteSystem");
        await noteMetadataJSON.SaveAsync(noteMetadataFS);
        var newVersion = await NoteVersion.CreateNewAsync(this, versionFS, Data);
        versions.Add(newVersion);
        NotifyPropertyChanged(nameof(CurrentVersion));
        return newVersion;
    }
    public partial Task AddTagAsync(Tag tag) => tag.AddNote(this);
    public partial Task RemoveTagAsync(Tag tag) => tag.RemoveNote(this);
    public partial Task DeleteAsync() => NoteSystem.DeleteNoteAsync(this);

}

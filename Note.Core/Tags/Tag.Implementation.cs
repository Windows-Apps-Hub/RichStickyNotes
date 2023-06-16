using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WAH.NoteSystem.Core.Notes;

namespace WAH.NoteSystem.Core.Tags;

public partial class Tag
{
    ObservableCollection<Note> notes = new();
    public partial async Task AddNote(Note note)
    {
        tagJSON.NotesId.AddLast(note.InternalId);
        await tagJSON.SaveAsync(TagFile);
        notes.Add(note);
        await note.InternalAddTagMetatdataAsync(this);
    }
    public partial async Task RemoveNote(Note note)
    {
        tagJSON.NotesId.Remove(note.InternalId);
        await tagJSON.SaveAsync(TagFile);
        notes.Remove(note);
        await note.InternalRemoveTagMetadataAsync(this);
    }

    public partial void Delete() => NoteSystem.DeleteTagAsync(this);
}
using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Storage;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core.Tags;

public partial class Tag
{
    readonly TagJSON tagJSON;
    readonly IFileStorage TagFile;
    private Tag(NoteSystem system, IFileStorage tagFile, TagJSON tagJSON)
    {
        NoteSystem = system;
        Notes = new(notes);
        TagFile = tagFile;
        this.tagJSON = tagJSON;
        var fn = TagFile.FileName;
        InternalId = fn[..fn.IndexOf(".")];
        if (system.InternalNoteInitialized) InternalFinalizeAddNotes();
    }

    internal static async Task<Tag> FromExistingAsync(
        NoteSystem system,
        IFileStorage tagFile
    ) => new(system, tagFile, await TagJSON.ReadAsync(tagFile));
    internal static async Task<Tag> CreateNewAsync(
        NoteSystem system,
        IFileStorage targetFile,
        TagJSON initialMetatadata
    )
    {
        await initialMetatadata.SaveAsync(targetFile);
        return new(system, targetFile, initialMetatadata);
    }
    internal void InternalFinalizeAddNotes()
    {
        foreach (var a in tagJSON.NotesId)
            notes.Add(NoteSystem.InternalGetNoteById(a));
    }
}

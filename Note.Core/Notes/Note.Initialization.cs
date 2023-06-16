using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Storage;
using WAH.NoteSystem.Core.Tags;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core.Notes;

public partial class Note
{
    private Note(NoteSystem parentSystem, IFolderStorage currFolder, IFolderStorage historyFolder, IFileStorage noteMetadataFS,
        NoteMetadataJSON noteMetadataJSON)
    {
        Tags = new(tags);
        Versions = new(versions);
        NoteFolder = currFolder;
        HistoryFolder = historyFolder;
        NoteSystem = parentSystem;
        this.noteMetadataJSON = noteMetadataJSON;
        this.noteMetadataFS = noteMetadataFS;
        InternalId = currFolder.FolderName;
    }
    internal static async Task<Note> FromExistingAsync(NoteSystem parentSystem, IFolderStorage currentFolder)
    {
        var historyfs = await currentFolder.GetFolderAsync("History");
        var metadatafs = await currentFolder.GetFileAsync("metadata.json");
        var note = new Note(parentSystem,
            currentFolder,
            historyfs,
            metadatafs,
            await NoteMetadataJSON.ReadAsync(metadatafs)
        );

        await foreach (var file in historyfs.GetFilesAsync())
        {
            var version = await NoteVersion.FromExistingAsync(note, file);
            note.versions.Add(version);
        }
        foreach (var tagId in note.noteMetadataJSON.TagIds)
        {
            var tag = note.NoteSystem.InternalGetTagById(tagId);
            note.tags.Add(tag);
        }
        return note;
    }
    internal static async Task<Note> CreateNewAsync(
        NoteSystem parentSystem,
        IFolderStorage currentFolder,
        NoteMetadataJSON noteMetadataJSON
    )
    {
        await noteMetadataJSON.SaveAsync(await currentFolder.GetFileAsync("metadata.json"));
        var note = await FromExistingAsync(parentSystem, currentFolder);
        await note.SaveNewVersionAsync(NoteData.CreateNewDefault());
        return note;
    }
    readonly ObservableCollection<Tag> tags = new();
    readonly ObservableCollection<NoteVersion> versions = new();
    readonly IFolderStorage NoteFolder;
    readonly NoteMetadataJSON noteMetadataJSON;
    readonly IFileStorage noteMetadataFS;
    readonly IFolderStorage HistoryFolder;
}
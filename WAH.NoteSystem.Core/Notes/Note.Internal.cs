using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Storage;
using WAH.NoteSystem.Core.Tags;
using System;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core.Notes;

public partial class Note
{
    internal string InternalId { get; }
    internal IFolderStorage InternalCurrentFolder => NoteFolder;
    internal async Task InternalAddTagMetatdataAsync(Tag tag)
    {
        tags.Add(tag);
        noteMetadataJSON.TagIds.AddLast(tag.InternalId);
        await noteMetadataJSON.SaveAsync(noteMetadataFS);
        if (tag == NoteSystem.StarTag) NotifyPropertyChanged(nameof(IsStarred));
    }
    internal async Task InternalRemoveTagMetadataAsync(Tag tag)
    {
        tags.Remove(tag);
        noteMetadataJSON.TagIds.Remove(tag.InternalId);
        await noteMetadataJSON.SaveAsync(noteMetadataFS);
        if (tag == NoteSystem.StarTag) NotifyPropertyChanged(nameof(IsStarred));
    }
    internal void InternalMarkAsInvalid() => IsValid = false;
}

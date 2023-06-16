using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core;

public partial class NoteSystem
{
    private ObservableCollection<Note> notes = new();
    private ObservableCollection<Tag> tags = new();

    /// <returns>A <see cref="Task"/> to wait until everything in note system is initialized</returns>
    public partial Task EnsureSystemInitializedAsync() => InitTCS.Task;
    /// <summary>
    /// Create new <see cref="Tag"/> and add to <see cref="Tags"/>
    /// </summary>
    /// <param name="tagName">The name for the <see cref="Tag"/></param>
    /// <returns>
    ///     <see cref="Task{TResult}"/> with the result of
    ///     newly registered <see cref="Tag"/> with the name <paramref name="tagName"/>
    /// </returns>
    public partial Task<Tag> RegsiterTagAsync(string tagName)
        => InternalRegisterTagAsync(new() { Name = tagName });
    internal partial async Task<Tag> InternalRegisterTagAsync(TagJSON tagJSON)
    {
        var file = await TagStorage.GetFileAsync($"{systemMetadataJSON.TagNextIndex++}.json");
        await file.EnsureSystemInitializedAsync();
        await systemMetadataJSON.SaveAsync(systemMetadataFS);
        var tag = await Tag.CreateNewAsync(this, file, tagJSON);
        tags.Add(tag);
        TagIdIndex.Add(tag.InternalId, tag);
        return tag;
    }
    /// <summary>
    /// Creates new <see cref="WAH.NoteSystem"/> and add to <see cref="Notes"/>
    /// </summary>
    /// <returns><see cref="Task{TResult}"/> with the result of newly created <see cref="WAH.NoteSystem"/>
    /// </returns>
    public partial Task<Note> CreateNewNoteAsync(string s) => CreateNewNoteAsync(s, null);
    /// <summary>
    /// Creates new <see cref="WAH.NoteSystem"/> and add to <see cref="Notes"/>
    /// </summary>
    /// <param name="tags">The tags for the <see cref="WAH.NoteSystem"/> to register</param>
    /// <returns><see cref="Task{TResult}"/> with the result of newly created <see cref="WAH.NoteSystem"/>
    /// </returns>
    public partial async Task<Note> CreateNewNoteAsync(string s, Tag? tag)
    {
        if (tag is not null) NotImplemented.Throw();
        var folder = await NoteStorage.GetFolderAsync(systemMetadataJSON.NoteNextIndex++.ToString());
        await systemMetadataJSON.SaveAsync(systemMetadataFS);
        var note = await Note.CreateNewAsync(this, folder, new() { Name = s });
        NoteIdIndex.Add(note.InternalId, note);
        notes.Add(note);
        return note!;
    }
    /// <summary>
    /// Removes <paramref name="note"/> from <see cref="Notes"/> and deletes all files associated.
    /// </summary>
    /// <param name="note">The <see cref="WAH.NoteSystem"/> to delete</param>
    /// <returns>An awaitable <see cref="Task{TResult}"/> that ends when the <paramref name="note"/> is fully deleted</returns>
    public partial async Task DeleteNoteAsync(Note note)
    {
        foreach (var tag in note.Tags.CacheAsArray())
        {
            await tag.RemoveNote(note);
        }
        NoteIdIndex.Remove(note.InternalId);
        note.InternalMarkAsInvalid();
        notes.Remove(note);
        await note.InternalCurrentFolder.DeleteAsync();
    }
    /// <summary>
    /// Removes the <paramref name="tag"/> from <see cref="Tags"/> and deletes all files associated.
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> to delete</param>
    /// <returns>An awaitable <see cref="Task{TResult}"/> that ends when the <paramref name="tag"/> is fully deleted</returns>
    public partial async Task DeleteTagAsync(Tag tag)
    {
        foreach (var note in tag.Notes.CacheAsArray())
        {
            await note.RemoveTagAsync(tag);
        }
        TagIdIndex.Remove(tag.InternalId);
        tag.InternalMarkAsInvalid();
        tags.Remove(tag);
        await tag.InternalCurrentFile.DeleteAsync();
    }

    private async Task EnsureDependencyInitialized()
    {
        await TagMainStorage.EnsureSystemInitializedAsync();
        await NoteMainStorage.EnsureSystemInitializedAsync();
        await TagStorage.EnsureSystemInitializedAsync();
        await NoteStorage.EnsureSystemInitializedAsync();
    }
}

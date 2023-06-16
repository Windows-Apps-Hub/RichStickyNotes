using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using WAH.NoteSystem.Core.Storage;
using WAH.NoteSystem.Core.JSON;
using System.Collections.Generic;
using System;
using Microsoft.UI;

namespace WAH.NoteSystem.Core;

public partial class NoteSystem
{
    readonly TaskCompletionSource InitTCS = new();

    public static readonly NoteSystem Current = new(ApplicationData.Current.LocalFolder);

    readonly IFolderStorage TagMainStorage, NoteMainStorage;
    IFolderStorage TagStorage, NoteStorage;
    NoteSystemMetadataJSON systemMetadataJSON;
    IFileStorage systemMetadataFS;
    public StorageFolder RootFolder { get; }
    private NoteSystem(StorageFolder rootFolder) {
        RootFolder = rootFolder;
        TagMainStorage = new FolderStorageManager(rootFolder, "Tag");
        NoteMainStorage = new FolderStorageManager(rootFolder, "Notes");
        Notes = new(notes);
        Tags = new(tags);
        StarTag = null!;
        TagStorage = null!;
        NoteStorage = null!;
        systemMetadataJSON = null!;
        systemMetadataFS = null!;
        Read(rootFolder);
    }
    readonly Dictionary<string, Tag> initTagStringIndex = new();
    readonly Dictionary<string, Tag> TagIdIndex = new();
    readonly Dictionary<string, Note> NoteIdIndex = new();
    internal bool InternalNoteInitialized { get; private set; } = false;
    async void Read(StorageFolder rootFolder)
    {
        try
        {
            var file = await rootFolder.CreateFileAsync("metadata.json", CreationCollisionOption.FailIfExists);
            systemMetadataJSON = new();
            systemMetadataFS = new FileStorageManager(file);
            await systemMetadataJSON.SaveAsync(systemMetadataFS);
        }
        catch // File already exists
        {
            systemMetadataFS = new FileStorageManager(rootFolder, "metadata.json");
        }
        await systemMetadataFS.EnsureSystemInitializedAsync();

        systemMetadataJSON = await NoteSystemMetadataJSON.ReadAsync(systemMetadataFS);

        await TagMainStorage.EnsureSystemInitializedAsync();
        TagStorage = await TagMainStorage.GetFolderAsync("Data");
        await NoteMainStorage.EnsureSystemInitializedAsync();
        NoteStorage = await NoteMainStorage.GetFolderAsync("Data");

        await EnsureDependencyInitialized();


        InternalNoteInitialized = false;

        await foreach (var tag in TagStorage.GetFilesAsync())
        {
            var tagobj = await Tag.FromExistingAsync(this, tag);
            tags.Add(tagobj);
            initTagStringIndex.Add(tagobj.Name, tagobj);
            TagIdIndex.Add(tagobj.InternalId, tagobj);
        }

        await foreach (var note in NoteStorage.GetFoldersAsync())
        {
            var noteobj = await Note.FromExistingAsync(this, note);
            notes.Add(noteobj);
            NoteIdIndex.Add(noteobj.InternalId, noteobj);
        }

        InternalNoteInitialized = true;

        foreach (var tag in tags)
        {
            tag.InternalFinalizeAddNotes();
        }
        if (!initTagStringIndex.TryGetValue("Star", out var star))
        {
            star = await InternalRegisterTagAsync(new TagJSON
            {
                Color = Colors.Gold,
                Name = "Star",
                Icon = SymbolEx.FavoriteStarFill
            });
        }
        StarTag = star;
        InitTCS.SetResult();
    }
}

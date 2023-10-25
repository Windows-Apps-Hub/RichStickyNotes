using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace WAH.NoteSystem.Core.Storage;

internal partial class FolderStorageManager : IFolderStorage
{
    TaskCompletionSource InitializeTCS = new();
    StorageFolder Folder;

    public string FolderName => Folder.Name;

    public FolderStorageManager(StorageFolder rootFolder, string FileName)
    {
        Folder = null!;
        InitAsync(rootFolder, FileName);
    }
    public FolderStorageManager(StorageFolder currentFolder)
    {
        Folder = currentFolder;
        InitializeTCS.SetResult();
    }
    private partial void InitAsync(StorageFolder rootFolder, string FolderName);
    private async partial void InitAsync(StorageFolder rootFolder, string FolderName)
    {
        try
        {
            Folder = await rootFolder.CreateFolderAsync(
                FolderName,
                CreationCollisionOption.OpenIfExists
            );
            InitializeTCS.SetResult();
        }
        catch (Exception e)
        {
            InitializeTCS.SetException(e);
        }
    }
    public Task EnsureSystemInitializedAsync() => InitializeTCS.Task;
    
    public async Task<IFolderStorage> GetFolderAsync(string name)
    {
        if (!InitializeTCS.Task.IsCompleted) throw new InvalidOperationException();
        var a = new FolderStorageManager(Folder, name);
        await a.EnsureSystemInitializedAsync();
        return a;
    }

    public async IAsyncEnumerable<IFolderStorage> GetFoldersAsync()
    {
        if (!InitializeTCS.Task.IsCompleted) throw new InvalidOperationException();
        foreach (var folder in await Folder.GetFoldersAsync())
        {
            yield return new FolderStorageManager(folder);
        }
    }

    public async Task<IFileStorage> GetFileAsync(string name)
    {
        if (!InitializeTCS.Task.IsCompleted) throw new InvalidOperationException();
        var a = new FileStorageManager(Folder, name);
        await a.EnsureSystemInitializedAsync();
        return a;
    }

    public async IAsyncEnumerable<IFileStorage> GetFilesAsync()
    {
        if (!InitializeTCS.Task.IsCompleted) throw new InvalidOperationException();
        foreach (var file in await Folder.GetFilesAsync())
        {
            yield return new FileStorageManager(file);
        }
    }

    public async Task DeleteAsync()
    {
        await Folder.DeleteAsync();
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WAH.NoteSystem.Core.Storage;

internal partial class FileStorageManager : IFileStorage
{
    TaskCompletionSource InitializeTCS = new();
    StorageFile File;
    StreamEx? Stream;

    public string FileName => File.Name;

    public async Task<DateTime> GetDateModifiedAsync()
    {
        var properties = await File.GetBasicPropertiesAsync();
        return properties.DateModified.DateTime;
    }

    //StorageLibraryChangeTracker tracker;
    public FileStorageManager(StorageFolder folder, string FileName)
    {
        File = null!;
        InitAsync(folder, FileName);
    }
    public FileStorageManager(StorageFile file)
    {
        File = file;
        InitializeTCS.SetResult();
    }
    private partial void InitAsync(StorageFolder folder, string FileName);
    public Task EnsureSystemInitializedAsync() => InitializeTCS.Task;
    private async partial void InitAsync(StorageFolder folder, string FileName)
    {
        try
        {
            File = await folder.CreateFileAsync(
                FileName,
                CreationCollisionOption.OpenIfExists
            );
            InitializeTCS.SetResult();
        }
        catch (Exception e)
        {
            InitializeTCS.SetException(e);
        }
    }

    public async Task<ReadOnlyStream> GetStreamForReadAsync()
    {
        if (Stream?.IsDisposed ?? true)
        {
            Stream = new((await File.OpenAsync(FileAccessMode.ReadWrite)).AsStream());
            Stream.Position = 0;
        }
        return new(Stream);
    }

    public async Task<StreamEx> GetStreamForWriteAsync()
    {
        if (Stream?.IsDisposed ?? true)
        {
            Stream = new((await File.OpenAsync(FileAccessMode.ReadWrite)).AsStream());
            Stream.Position = 0;
        }
        return new(Stream);
    }

    public async Task RenameAsync(string newFileName)
    {
        await File.RenameAsync(newFileName);
    }

    public async Task DeleteAsync()
    {
        await File.DeleteAsync();
    }
}
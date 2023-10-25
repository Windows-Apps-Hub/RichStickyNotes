//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Windows.Foundation;
//using Windows.Storage;
//using Windows.Storage.Streams;

//namespace NoteManager.Storage;

//internal partial class SafeFileStorageManager : IFileStorage
//{
//    IFolderStorage Folder;
//    StorageFile File;
//    public SafeFileStorageManager(IFolderStorage folderStorage)
//    {
//        Folder = folderStorage;
//    }
//    public Task EnsureSystemInitialized() => Task.CompletedTask;

//    public Task<ReadOnlyStream> GetStreamForReadAsync()
//    {
//        throw new NotImplementedException();
//    }

//    public Task<StreamEx> GetStreamForWriteAsync()
//    {
//        throw new NotImplementedException();
//    }
//}
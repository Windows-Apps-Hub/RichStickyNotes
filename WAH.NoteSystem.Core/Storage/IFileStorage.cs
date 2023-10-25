using System;
using System.IO;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core.Storage;

interface IFileStorage
{
    string FileName { get; }
    Task RenameAsync(string newFileName);
    Task EnsureSystemInitializedAsync();
    Task<ReadOnlyStream> GetStreamForReadAsync();
    Task<StreamEx> GetStreamForWriteAsync();
    Task<DateTime> GetDateModifiedAsync();
    Task DeleteAsync();
}

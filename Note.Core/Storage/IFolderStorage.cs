using System.Collections.Generic;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core.Storage;

interface IFolderStorage
{
    string FolderName { get; }
    Task EnsureSystemInitializedAsync();
    Task<IFolderStorage> GetFolderAsync(string name);
    IAsyncEnumerable<IFolderStorage> GetFoldersAsync();
    Task<IFileStorage> GetFileAsync(string name);
    IAsyncEnumerable<IFileStorage> GetFilesAsync();
    Task DeleteAsync();
}
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WAH.NoteSystem.Core.Storage;

namespace WAH.NoteSystem.Core.Notes;
public class NoteVersion : INotifyPropertyChanged
{
    internal const string KeepForeverPrefix = "KeepForever";
    public DateTime Time { get; }
    public bool KeepForever {
        get => file.FileName.StartsWith(KeepForeverPrefix);
        set
        {
            if (value == KeepForever) return;
            var dq = DispatcherQueue.GetForCurrentThread();
            Task.Run(async delegate
            {
                if (value)
                    await file.RenameAsync($"{KeepForeverPrefix}{file.FileName}");
                else
                    await file.RenameAsync(file.FileName[KeepForeverPrefix.Length..]);
                dq.TryEnqueue(delegate
                {
                    PropertyChanged?.Invoke(this, new(nameof(KeepForever)));
                });
            });
        }
    }
    public Note AssociatedNote { get; }
    public Task<NoteData> GetDataAsync()
    {
        return NoteData.ReadAsync(file);
    }
    IFileStorage file;

    public event PropertyChangedEventHandler? PropertyChanged;

    private NoteVersion(Note AssociatedNote, DateTime dateModified, IFileStorage fileStorageManager) {
        this.AssociatedNote = AssociatedNote;
        //Data = noteData;
        Time = dateModified;
        file = fileStorageManager;
    }
    internal static async Task<NoteVersion> FromExistingAsync(Note AssociatedNote, IFileStorage fileStorageManager)
    {
        return new(AssociatedNote, await fileStorageManager.GetDateModifiedAsync(), fileStorageManager);
    }
    internal static async Task<NoteVersion> CreateNewAsync(Note AssociatedNote, IFileStorage fileStorageManager, NoteData noteData)
    {
        await noteData.SaveAsync(fileStorageManager);
        return await FromExistingAsync(AssociatedNote, fileStorageManager);
    }
}
using System.ComponentModel;

namespace WAH.NoteSystem.Core.Notes;

public partial class Note : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new(propertyName));
}
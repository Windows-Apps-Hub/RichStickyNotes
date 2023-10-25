using System.ComponentModel;

namespace WAH.NoteSystem.Core.Tags;

public partial class Tag : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new(propertyName));
}
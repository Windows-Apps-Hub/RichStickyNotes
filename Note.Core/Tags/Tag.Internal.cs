using WAH.NoteSystem.Core.Storage;

namespace WAH.NoteSystem.Core.Tags;

public partial class Tag
{
    internal string InternalId { get; }
    internal IFileStorage InternalCurrentFile => TagFile;
    internal partial void InternalMarkAsInvalid();
    internal partial void InternalMarkAsInvalid() => IsValid = false;
}

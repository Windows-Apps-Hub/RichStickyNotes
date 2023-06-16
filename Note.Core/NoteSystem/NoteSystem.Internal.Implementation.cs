using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;

namespace WAH.NoteSystem.Core;

public partial class NoteSystem
{
    internal partial Tag InternalGetTagById(string tagId)
    {
        return TagIdIndex[tagId];
    }
    internal partial Note InternalGetNoteById(string noteId)
    {
        return NoteIdIndex[noteId];
    }
}

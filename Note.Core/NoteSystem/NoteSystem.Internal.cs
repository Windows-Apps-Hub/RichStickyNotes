using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core;

public partial class NoteSystem
{
    internal partial Tag InternalGetTagById(string tagId);
    internal partial Note InternalGetNoteById(string noteId);
    internal partial Task<Tag> InternalRegisterTagAsync(TagJSON tagJSON);
}

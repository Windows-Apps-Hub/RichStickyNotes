using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using WAH.NoteSystem.Core.Notes;
using WAH.NoteSystem.Core.Tags;
using WAH.NoteSystem.Core.Storage;
using System.Diagnostics;

namespace WAH.NoteSystem.Core;
/// <summary>
/// The system to manage a note of a specific directory
/// </summary>
[DebuggerDisplay("NoteSystem with {Notes.Count} notes and {Tags.Count} tags")]
public partial class NoteSystem
{
    /// <summary>
    /// All the <see cref="WAH.Note"/>s stored in the current <see cref="NoteSystem"/>
    /// </summary>
    public ReadOnlyObservableCollection<Note> Notes { get; }
    /// <summary>
    /// All the <see cref="Tag"/>s stored in the current <see cref="NoteSystem"/>
    /// </summary>
    public ReadOnlyObservableCollection<Tag> Tags { get; }
    /// <summary>
    /// The predefined Star <see cref="Tag"/>
    /// </summary>
    public Tag StarTag { get; private set; }
    
    public partial Task EnsureSystemInitializedAsync();
    public partial Task<Tag> RegsiterTagAsync(string tagName);
    public partial Task<Note> CreateNewNoteAsync(string s);
    public partial Task<Note> CreateNewNoteAsync(string s, Tag? tag);
    public partial Task DeleteNoteAsync(Note note);
    public partial Task DeleteTagAsync(Tag tag);
}

using System.IO;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    struct Nothing { }
    class TaskCompletionSource : TaskCompletionSource<Nothing>
    {
        public void SetResult() => SetResult(default);
    }
}
namespace WAH.NoteSystem.Core
{
    static partial class Extension
    {
        public static Task DisposeAsync(this Stream stream)
            => Task.Run(stream.Dispose);
    }
}
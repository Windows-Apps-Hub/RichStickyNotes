using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core;

static class NotImplemented
{
    [DoesNotReturn]
    public static void Throw()
    {
        throw new NotImplementedException();
    }
    [DoesNotReturn]
    public static T ThrowReturn<T>()
    {
        throw new NotImplementedException();
    }
    [DoesNotReturn]
    public static Task ThrowAsync()
    {
        throw new NotImplementedException();
    }
    [DoesNotReturn]
    public static Task<T> ThrowAsync<T>()
    {
        throw new NotImplementedException();
    }
    [DoesNotReturn]
    public static IEnumerable<T> ThrowReturnEnumerable<T>()
    {
        throw new NotImplementedException();
    }
    [DoesNotReturn]
    public static IAsyncEnumerable<T> ThrowAsyncEnumerable<T>()
    {
        throw new NotImplementedException();
    }
}

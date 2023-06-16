using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAH.NoteSystem.Core;

internal static class Extension
{
    public static T[] CacheAsArray<T>(this IEnumerable<T> ts) => ts.ToArray();
}

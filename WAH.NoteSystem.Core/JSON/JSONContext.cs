using System.Text.Json.Serialization;
using WAH.NoteSystem.Core.Notes;

namespace WAH.NoteSystem.Core.JSON;

[JsonSerializable(typeof(TagJSON))]
[JsonSerializable(typeof(NameJSON))]
[JsonSerializable(typeof(NoteMetadataJSON))]
[JsonSerializable(typeof(NoteSystemMetadataJSON))]
[JsonSerializable(typeof(NoteData))]
internal partial class MyJsonContext : JsonSerializerContext
{
}
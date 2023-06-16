using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using WAH.NoteSystem.Core.Storage;
using System.Text.Json;
using Microsoft.UI;

namespace WAH.NoteSystem.Core.JSON;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(TagJSON))]
internal class TagJSON
{
    public string Name { get; set; } = "";
    public LinkedList<string> NotesId { get; set; } = new();
    public Windows.UI.Color Color { get; set; } = Colors.White;
    public SymbolEx Icon { get; set; } = SymbolEx.Tag;
    public async Task SaveAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForWriteAsync();
        stream.Position = 0;
        JsonSerializer.Serialize(
            stream,
            this,
            MyJsonContext.Default.TagJSON
        );
        stream.SetLength(stream.Position);
        await stream.FlushAsync();
        await stream.DisposeAsync();
    }
    public static async Task<TagJSON> ReadAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForReadAsync();
        stream.Position = 0;
        var output = JsonSerializer.Deserialize(
            stream,
            MyJsonContext.Default.TagJSON
        );
        await stream.DisposeAsync();
        if (output is null) throw new InvalidOperationException();
        return output;
    }
}
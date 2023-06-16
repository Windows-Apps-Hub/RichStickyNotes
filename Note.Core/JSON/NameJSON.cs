using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using WAH.NoteSystem.Core.Storage;
using System.Text.Json;

namespace WAH.NoteSystem.Core.JSON;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(NameJSON))]
internal class NameJSON
{
    public SortedDictionary<string /* FileName */, string /* Actual Name */> NamesDict { get; set; } = new();
    
    public async Task SaveAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForWriteAsync();
        stream.Position = 0;
        JsonSerializer.Serialize(
            stream,
            this,
            MyJsonContext.Default.NameJSON
        );
        stream.SetLength(stream.Position);
        await stream.FlushAsync();
        await stream.DisposeAsync();
    }
    public static async Task<NameJSON> ReadAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForReadAsync();
        stream.Position = 0;
        var output = JsonSerializer.Deserialize(
            stream,
            MyJsonContext.Default.NameJSON
        );
        await stream.DisposeAsync();
        if (output is null) throw new InvalidOperationException();
        return output;
    }
}
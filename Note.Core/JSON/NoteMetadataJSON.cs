using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using WAH.NoteSystem.Core.Storage;
using System.Text.Json;
using System.Collections.Generic;
using Windows.UI;

namespace WAH.NoteSystem.Core.JSON;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(NoteMetadataJSON))]
internal class NoteMetadataJSON
{
    public ulong NextVersionIndex { get; set; } = 1;
    public string Name { get; set; } = "";
    public Color Color { get; set; } = Color.FromArgb(255 * 10 / 100, 254, 247, 210);
    public LinkedList<string> TagIds { get; set; } = new();
    public async Task SaveAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForWriteAsync();
        JsonSerializer.Serialize(
            stream,
            this,
            MyJsonContext.Default.NoteMetadataJSON
        );
        stream.SetLength(stream.Position);
        await stream.FlushAsync();
        await stream.DisposeAsync();
    }
    public static async Task<NoteMetadataJSON> ReadAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForReadAsync();
        stream.Position = 0;
        var output = JsonSerializer.Deserialize(
            stream,
            MyJsonContext.Default.NoteMetadataJSON
        );
        await stream.DisposeAsync();
        if (output is null) throw new InvalidOperationException();
        return output;
    }
}
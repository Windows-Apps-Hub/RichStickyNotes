using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using WAH.NoteSystem.Core.Storage;
using System.Text.Json;
using System.Collections.Generic;

namespace WAH.NoteSystem.Core.JSON;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(NoteSystemMetadataJSON))]
internal class NoteSystemMetadataJSON
{
    public int NoteNextIndex { get; set; } = 1;
    public int TagNextIndex { get; set; } = 1;
    public async Task SaveAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForWriteAsync();
        stream.Position = 0;
        JsonSerializer.Serialize(
            stream,
            this,
            MyJsonContext.Default.NoteSystemMetadataJSON
        );
        stream.SetLength(stream.Position);
        await stream.FlushAsync();
        await stream.DisposeAsync();
    }
    public static async Task<NoteSystemMetadataJSON> ReadAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForReadAsync();
        stream.Position = 0;
        var output = JsonSerializer.Deserialize(
            stream,
            MyJsonContext.Default.NoteSystemMetadataJSON
        );
        await stream.DisposeAsync();
        if (output is null) throw new InvalidOperationException();
        return output;
    }
}
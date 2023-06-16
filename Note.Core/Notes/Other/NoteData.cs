using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using WAH.NoteSystem.Core.JSON;
using WAH.NoteSystem.Core.Storage;
using Windows.Networking.Sockets;

namespace WAH.NoteSystem.Core.Notes;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(NoteData))]
public class NoteData
{
    internal static NoteData CreateNewDefault() => new() { Cells = { new RTFCell( ) } };

    public List<NoteCellBase> Cells { get; set; } = new();

    internal async Task SaveAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForWriteAsync();
        stream.Position = 0;
        JsonSerializer.Serialize(
            stream,
            this,
            MyJsonContext.Default.NoteData
        );
        stream.SetLength(stream.Position);
        await stream.FlushAsync();
        await stream.DisposeAsync();
    }
    internal static async Task<NoteData> ReadAsync(IFileStorage fileStorageManager)
    {
        var stream = await fileStorageManager.GetStreamForReadAsync();
        stream.Position = 0;
        var output = JsonSerializer.Deserialize(
            stream,
            MyJsonContext.Default.NoteData
        );
        await stream.DisposeAsync();
        if (output is null) throw new InvalidOperationException();
        return output;
    }
}
[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonDerivedType(typeof(RTFCell), typeDiscriminator: nameof(RTFCell))]
public class NoteCellBase
{

}
[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
public class RTFCell : NoteCellBase
{
    public string RTF { get; set; } = "";
}
using System;
using System.IO;
using Windows.Storage.Streams;

namespace WAH.NoteSystem.Core.Storage;


class StreamEx : Stream
{
    public bool IsDisposed { get; private set; } = false;
    protected Stream Stream;
    internal void UnsafeSwapStream(Stream newStream)
        => Stream = newStream;
    internal Stream UnsafeGetStream()
        => Stream;
    public StreamEx(Stream stream)
    {
        Stream = stream;
    }
    public override bool CanRead => Stream.CanRead;

    public override bool CanSeek => Stream.CanSeek;

    public override bool CanWrite => Stream.CanWrite;

    public override long Length => Stream.Length;

    public override long Position { get => Stream.Position; set => Stream.Position = value; }

    public override void Flush()
    {
        Stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return Stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        Stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Stream.Write(buffer, offset, count);
    }
    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        Stream.Dispose();
    }
}

class ReadOnlyStream : StreamEx
{
    public ReadOnlyStream(Stream stream) : base(stream)
    {
    }
    public override bool CanWrite => false;

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException();
    }
}
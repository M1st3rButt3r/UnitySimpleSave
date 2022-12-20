using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public enum CompressionMethod
{
    None,
    GZip
}

public static class SimpleCompression
{
    public static readonly Dictionary<CompressionMethod, Func<Stream, Stream>> Writer =
        new Dictionary<CompressionMethod, Func<Stream, Stream>>
        {
            {CompressionMethod.None, stream => stream},
            {CompressionMethod.GZip, GetCompressionStream},
        };
    
    public static readonly Dictionary<CompressionMethod, Func<Stream, Stream>> Reader =
        new Dictionary<CompressionMethod, Func<Stream, Stream>>
        {
            {CompressionMethod.None, stream => stream},
            {CompressionMethod.GZip, GetDecompressionStream},
        };
    
    
    public static Stream GetCompressionStream(this Stream stream)
    {
        return new GZipStream(stream, CompressionMode.Compress);
    }

    public static Stream GetDecompressionStream(this Stream stream)
    {
        return new GZipStream(stream, CompressionMode.Decompress);
    }
    
    public static Stream Compress(this Stream stream, CompressionMethod compressionMethod)
    {
        return Writer[compressionMethod].Invoke(stream);
    }

    public static Stream Decompress(this Stream stream, CompressionMethod compressionMethod)
    {
        return Reader[compressionMethod].Invoke(stream);
    }
}

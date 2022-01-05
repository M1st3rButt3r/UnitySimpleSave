using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum EncryptionMethod
{
    None,
    Aes
}

public static class SimpleEncryption
{
    public static byte[] key
    {
        get
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(SimpleSaveSettings.GetOrCreateSettings().password));
        }
    }
    
    public static readonly Dictionary<EncryptionMethod, Func<Stream, Stream>> Writer =
        new Dictionary<EncryptionMethod, Func<Stream, Stream>>
        {
            {EncryptionMethod.None, stream => stream},
            {EncryptionMethod.Aes, GetAesWriterStream},
        };
    
    public static readonly Dictionary<EncryptionMethod, Func<Stream, Stream>> Reader =
        new Dictionary<EncryptionMethod, Func<Stream, Stream>>
        {
            {EncryptionMethod.None, stream => stream},
            {EncryptionMethod.Aes, GetAesReaderStream},
        };

    public static Stream GetAesWriterStream(Stream stream)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        stream.Write(aes.IV, 0, aes.IV.Length);
        
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        
        return new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
    }

    public static Stream GetAesReaderStream(Stream stream)
    {
        Aes aes = Aes.Create();
        aes.Key = key;
        
        byte[] iv = new byte[aes.BlockSize / 8];
        int remain = iv.Length;
        while (remain != 0)
        {
            int read = stream.Read(iv, iv.Length - remain, remain);

            remain -= read;
        }
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        
        
        return new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
    }
}
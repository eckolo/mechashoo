using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Crypt
{
    private const string AES_IV = @"gtFZy36D3t6cGRwh";
    private const string AES_KEY = @"pPQixgZaEolfNYjt";

    /// <summary>
    /// 文字列を暗号化する
    /// </summary>
    /// <param name="text">平文</param>
    /// <returns>暗号文</returns>
    public static string EncodeCrypt(this string text)
    {
        var aes = new RijndaelManaged()
        {
            BlockSize = 128,
            KeySize = 128,
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.CBC,
            Key = System.Text.Encoding.UTF8.GetBytes(AES_KEY),
            IV = System.Text.Encoding.UTF8.GetBytes(AES_IV)
        };
        ICryptoTransform encrypt = aes.CreateEncryptor();
        var memoryStream = new MemoryStream();
        var cryptStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);

        byte[] text_bytes = System.Text.Encoding.UTF8.GetBytes(text);

        cryptStream.Write(text_bytes, 0, text_bytes.Length);
        cryptStream.FlushFinalBlock();

        byte[] encrypted = memoryStream.ToArray();

        return (System.Convert.ToBase64String(encrypted));
    }

    /// <summary>
    /// 暗号文を解読する
    /// </summary>
    /// <param name="cryptText">暗号文</param>
    /// <returns>平文</returns>
    public static string DecodeCrypt(this string cryptText)
    {
        var aes = new RijndaelManaged()
        {
            BlockSize = 128,
            KeySize = 128,
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.CBC,
            Key = System.Text.Encoding.UTF8.GetBytes(AES_KEY),
            IV = System.Text.Encoding.UTF8.GetBytes(AES_IV)
        };
        ICryptoTransform decryptor = aes.CreateDecryptor();

        byte[] encrypted = System.Convert.FromBase64String(cryptText);
        byte[] planeText = new byte[encrypted.Length];

        var memoryStream = new MemoryStream(encrypted);
        var cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        cryptStream.Read(planeText, 0, planeText.Length);

        return (System.Text.Encoding.UTF8.GetString(planeText));
    }
}
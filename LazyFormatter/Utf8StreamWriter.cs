using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace WMA
{
  internal static class Utf8StreamWriter
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(T? value, Stream target, Encoding encoding)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      var maxLenght = CalculateLenght(value, out var isString);
      var bytesMax = encoding.GetMaxByteCount(maxLenght);
      if(bytesMax < 1024)
      {
        Span<char> charBuffer = stackalloc char[maxLenght];
        Span<byte> byteBuffer = stackalloc byte[bytesMax];
        var bytesWritten = 0;
        if(isString)
        {
          bytesWritten = encoding.GetBytes(value.ToString().AsSpan(), byteBuffer);
        }
        else if (value is ISpanFormattable sf && sf.TryFormat(charBuffer, out var charsWritten, Span<char>.Empty, CultureInfo.InvariantCulture) && charsWritten > 0)
        {
          bytesWritten = encoding.GetBytes(charBuffer.Slice(0, charsWritten), byteBuffer);
        }
        if (bytesWritten > 0)
        {
          target.Write(byteBuffer.Slice(0, bytesWritten));
        }
      }
      else
      {
        throw new NotSupportedException("Values longer than 1024 characters are not supported yet");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateLenght<T>(T? value, out bool isString)
    {
      isString = false;
      if(value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      if(value is int || value is long || value is double)
      {
        return 20;
      }
      else if(value is decimal)
      {
        return 30;
      }
      else if (value is float)
      {
        return 10;
      }
      else if (value is bool)
      {
        return 5;
      }
      else if(value is char)
      {
        return 1;
      }
      isString = true;
      return value.ToString()!.Length;
    }
  }
}

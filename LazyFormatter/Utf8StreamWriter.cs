using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace WMA
{
  internal static class Utf8StreamWriter
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(T value, Stream target, Encoding encoding)
    {
      ArgumentNullException.ThrowIfNull(value);

      string? stringVal = null;
      var maxLenght = TryCalculateLength<T>(out int? length) ? length.Value : (stringVal = value.ToString()!).Length;
      var bytesMax = encoding.GetMaxByteCount(maxLenght);
      if(bytesMax < 1024)
      {
        Span<char> charBuffer = stackalloc char[maxLenght];
        Span<byte> byteBuffer = stackalloc byte[bytesMax];
        var bytesWritten = 0;
        if(stringVal is not null)
        {
          bytesWritten = encoding.GetBytes(stringVal, byteBuffer);
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
    private static bool TryCalculateLength<T>([NotNullWhen(true)] out int? length)
    {
      if(typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(double))
      {
        length = 20;
        return true;
      }
      else if(typeof(T) == typeof(decimal))
      {
        length = 30;
        return true;
      }
      else if (typeof(T) == typeof(float))
      {
        length = 10;
        return true;
      }
      else if (typeof(T) == typeof(bool))
      {
        length = 5;
        return true;
      }
      else if(typeof(T) == typeof(char))
      {
        length = 1;
        return true;
      }
      length = null;
      return false;
    }
  }
}

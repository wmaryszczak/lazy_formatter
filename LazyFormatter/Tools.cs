using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMA
{
  internal static class Tools
  {
    public static bool TryToInt32(this byte[] value, int index, int count, out int result)
    {
      return value.AsSpan().Slice(index, count).TryToInt32(out result);
    }
    public static bool TryToInt32(this Span<byte> value, out int result)
    {
      result = 0;
      if (value.IsEmpty)
      {
        return false;
      }

      if (value[0] == '-')
      {
        for (int i = 1; i < value.Length; i++)
        {
          result = (result * 10) - (value[i] - '0');
        }
      }
      else
      {
        for (int i = 0; i < value.Length; i++)
        {
          result = (result * 10) + (value[i] - '0');
        }
      }
      return true;
    }
  }
}

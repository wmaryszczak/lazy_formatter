using System.Text;

namespace WMA;

public class Utf8LazyFormatter
{
  private readonly (int placeholderIndex, int beginIndex, int endIndex)[] indexes;
  private readonly byte[] utf8Content;
  private readonly byte patternStart;
  private readonly byte patternEnd;
  private readonly System.Text.Encoding enncoding = System.Text.Encoding.UTF8;

  public static Utf8LazyFormatter Create(string pattern, char patternStart, char patternEnd)
  {
    return new Utf8LazyFormatter(
      System.Text.Encoding.UTF8.GetBytes(pattern),
      Convert.ToByte(patternStart),
      Convert.ToByte(patternEnd)
    );
  }

  public Utf8LazyFormatter(byte[] utf8Content, byte patternStart, byte patternEnd)
  {
    this.utf8Content = utf8Content;
    this.patternStart = patternStart;
    this.patternEnd = patternEnd;
    this.indexes = ScanContentForPlaceholders(utf8Content);
  }

  private (int, int, int)[] ScanContentForPlaceholders(byte[] utf8Content)
  {
    var indexes = new List<(int, int, int)> { };
    int idx1 = 0;
    int idx2 = 0;
    while (true)
    {
      int newIdx1 = Array.IndexOf<byte>(utf8Content, this.patternStart, idx1);
      int newIdx2 = Array.IndexOf<byte>(utf8Content, this.patternEnd, idx2);
      if (newIdx1 != -1 && newIdx2 != -1)
      {
        if(newIdx1 - idx2 > 0)
        {
          indexes.Add((-1, idx2, newIdx1));
        }
        if(utf8Content.TryToInt32(newIdx1 + 1, newIdx2 - (newIdx1 + 1), out var placeHolderIndex))
        {
          indexes.Add((placeHolderIndex, newIdx1, newIdx2));
        }
        else
        {
          throw new FormatException();
        }
        // move idx1 after newIdx2, interpolation pattern cannot overlap
        idx1 = newIdx2;
        idx2 = newIdx2 + 1;
      }
      else
      {
        break;
      }
    }

    return indexes.ToArray();
  }

  public void Format<T1>(Stream target, T1 val1)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    formatter.AssignValue(val1, 0);
    formatter.FormatAll();
  }

  public void Format<T1, T2>(Stream target, T1 val1, T2 val2)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    formatter.AssignValue(val1, 0);
    formatter.AssignValue(val2, 1);
    formatter.FormatAll();
  }

  public void Format<T1, T2, T3>(Stream target, T1 val1, T2 val2, T3 val3)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    formatter.AssignValue(val1, 0);
    formatter.AssignValue(val2, 1);
    formatter.AssignValue(val3, 2);
    formatter.FormatAll();
  }

  public void Format<T1, T2, T3, T4>(Stream target, T1 val1, T2 val2, T3 val3, T4 val4)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    formatter.AssignValue(val1, 0);
    formatter.AssignValue(val2, 1);
    formatter.AssignValue(val3, 2);
    formatter.AssignValue(val4, 3);
    formatter.FormatAll();
  }

  public void Format(Stream target, ReadOnlySpan<object> values)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    for (int i = 0; i < values.Length; i++)
    {
      formatter.AssignValue(values[i], i);
    }
    formatter.FormatAll();
  }

  public void Format(Stream target, params object[] values)
  {
    var formatter = new Utf8LazyFormatterHandler(this.indexes, this.utf8Content, target, this.enncoding);
    for (int i = 0; i < values.Length; i++)
    {
      formatter.AssignValue(values[i], i);
    }
    formatter.FormatAll();
  }
}

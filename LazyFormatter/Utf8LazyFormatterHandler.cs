using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace WMA
{
  internal readonly struct Utf8LazyFormatterHandler
  {
    private readonly (int placeholderIndex, int beginIndex, int endIndex)[] indexes;
    private readonly byte[] utf8Content;
    private readonly Stream utf8TargetStream;
    private readonly Encoding enncoding;
    private readonly List<Action<Stream, Encoding>> formatFunctions;

    public Utf8LazyFormatterHandler(
      (int placeholderIndex, int beginIndex, int endIndex)[] indexes,
      byte[] utf8Content,
      Stream utfTarget8Stream,
      Encoding enncoding
    )
    {
      this.indexes = indexes;
      this.utf8Content = utf8Content;
      this.utf8TargetStream = utfTarget8Stream;
      this.enncoding = enncoding;
      this.formatFunctions = new(indexes.Length);
    }

    public void AssignValue<T>(T val, int index)
    {
      this.formatFunctions.Insert(index, (_targetStream, _encoding) =>
      {
        Utf8StreamWriter.Write(val, _targetStream, _encoding);
      });
    }

    public void FormatAll()
    {
      var currentContentIdx = 0;
      for (int i = 0; i < this.indexes.Length; i++)
      {
        var placeholder = this.indexes[i];
        if (!GetFormatFunction(placeholder.placeholderIndex, out var formatFunction))
        {
          var (_, beginIdx, endIdx) = placeholder;
          this.utf8TargetStream.Write(this.utf8Content, beginIdx, endIdx - beginIdx);
        }
        else
        {
          formatFunction(this.utf8TargetStream, this.enncoding);
        }
        currentContentIdx = placeholder.endIndex + 1;
      }
      if (currentContentIdx < this.utf8Content.Length) // something has left
      {
        this.utf8TargetStream.Write(this.utf8Content, currentContentIdx, this.utf8Content.Length - currentContentIdx);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool GetFormatFunction(int placeholderIndex, [NotNullWhen(true)] out Action<Stream, Encoding>? formatFunction)
    {
      if (placeholderIndex != -1 && placeholderIndex < this.formatFunctions.Count)
      {
        formatFunction = this.formatFunctions[placeholderIndex];
        return true;
      }
      else
      {
        formatFunction = null;
        return false;
      }
    }
  }
}

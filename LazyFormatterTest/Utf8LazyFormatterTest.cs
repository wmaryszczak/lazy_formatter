using WMA;

namespace LazyFormatterTest;

public class Utf8LazyFormatterTest
{
  [Theory]
  [InlineData("<test>{0}</test>", "<test>1234</test>", "1234")]
  [InlineData("<test>{1}</test>", "<test>4321</test>", "1234", "4321")]
  [InlineData("{0}<test>{1}</test><tes2>{2}<test2>{3}", "<html><test>my test value</test><tes2>wma<test2></html>", "<html>", "my test value", "wma", "</html>")]
  [InlineData("{3}<test>{1}</test><tes2>{2}<test2>{3}", "</html><test>my test value</test><tes2>wma<test2></html>", "<html>", "my test value", "wma", "</html>")]
  [InlineData(
    "{3}{1}<test>test</test><tes2>{2}<test2>{3}",
    "</html>my test value<test>test</test><tes2>wma<test2></html>", "<html>", "my test value", "wma", "</html>")]
  public void Should_Format_Various_User_Cases(string pattern, string expected, params object[] args)
  {
    var subject = Utf8LazyFormatter.Create(
      pattern, '{', '}'
    );

    using var stream = new MemoryStream();
    subject.Format(stream, args);
    stream.Position = 0;
    var actual = new StreamReader(stream).ReadToEnd();
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Should_Format_Non_String_Values()
  {
    var pattern = "<test>{2}</test>{1}<test2>{0}<test2>";
    var expected = "<test>11/11/2022 00:00:00</test>12234.222<test2>234<test2>";
    var subject = Utf8LazyFormatter.Create(
      pattern, '{', '}'
    );

    using var stream = new MemoryStream();
    subject.Format(stream, 234, 12234.222, new DateTime(2022,11,11));
    stream.Position = 0;
    var actual = new StreamReader(stream).ReadToEnd();
    Assert.Equal(expected, actual);
  }

  [Fact]
  public async Task Should_Format_Thread_Safe()
  {
    var pattern = "{0} <test>{1}</test>{2}<test2>{3}<test2>";
    var subject = Utf8LazyFormatter.Create(
      pattern, '{', '}'
    );
    var taskList = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
      {
        using var stream = new MemoryStream();
        subject.Format(stream, Task.CurrentId, 234, 12234.222, new DateTime(2022, 11, 11));
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        return actual;
      })).ToList();

    await Task.WhenAll(taskList);

    foreach(var t in taskList)
    {
      var expected = t.Id + " <test>234</test>12234.222<test2>11/11/2022 00:00:00<test2>";
      Assert.Equal(expected, await t);
    }
  }
}

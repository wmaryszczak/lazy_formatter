# lazy_formatter

The `LazyFormatter` has the following features:

* Format content with variables into stream without writing complex xml or json writing routines.
* Separate format declaration from the execution.
* The `Utf8LazyFormatter.Format` method has similar interface to `string.Format` method
* It reduces the memory footprint to minimum.
* The `Utf8LazyFormatter.Format` method is thread-safe, can be called by multiple threads at once.

## How to use it

Include the nuget package in the project

`https://www.nuget.org/packages/LazyFormatter`

Declare once as singleton or static variable:

```c#
    var pattern = "<test>{2}</test>{1}<test2>{0}<test2>";
    var formatter = Utf8LazyFormatter.Create(
      pattern, '{', '}'
    );
```

Use multiple times with variables provided as input to format method

```c#
  var arr = ArrayPool<byte>.Shared.Rent(pattern.Length * 2)
  try
  {
    using var stream = new MemoryStream(arr);
    formatter.Format(stream, 234, 12234.222, new DateTime(2022,11,11));
    // do something with stream
  }
  finally
  {
    ArrayPool<byte>.Shared.Return(arr);
  }
```

## Benchmark tests

To run locally use
```bash
cd LazyFormatterTest && dotnet run -c Release
```

There is also set-up continuous benchmarking as GitHub action.
The results are available [here](https://wmaryszczak.github.io/lazy_formatter/dev/bench/).

## How to run

run unit tests

```bash
dotnet test
```

run Benchmark.Net

```bash
cd LazyFormatterTest
dotnet run -c Release -- --job short --runtimes net7.0 --filter "*"
```

## TODO:

* Provide format into `{1}` placeholders like `{1:C2}`

# lazy_formatter

The `LazyFormatter` has the following features:

* Format content with variables into stream without writing complex xml or json writing routines.
* Separate format declaration from the execution.
* The `Utf8LazyFormatter.Format` method has similar interface to `string.Format` method
* It reduces the memory footprint to minumum.
* The `Utf8LazyFormatter.Format` method is thread-safe, can be called by multiple threads at once.

## How to use it

Declare once as singleton or static variable:

```
    var pattern = "<test>{2}</test>{1}<test2>{0}<test2>";
    var formatter = Utf8LazyFormatter.Create(
      pattern, '{', '}'
    );
```

Use multiple times with variables provided as input to format method

```
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

## Benchmarks

| Method                       |     Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|------------------------------|---------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| Benchmark_Formatter          | 352.5 ns | 150.7 ns |  8.26 ns |  1.00 |    0.00 | 0.0787 |      - |     496 B |        1.00 |
| Benchmrk_StringInterpolation | 465.7 ns | 915.2 ns | 50.17 ns |  1.32 |    0.12 | 0.6332 | 0.0086 |    3976 B |        8.02 |


## How to run

run unit tests

```
dotnet test
```

run Benchmark.Net

```
cd LazyFormatterTest
dotnet run -c Release -- --job short --runtimes net7.0 --filter "*"
```

## TODO:

* Provide format into `{1}` placeholders like `{1:C2}`

using System.Text;
using BenchmarkDotNet.Attributes;
using WMA;

namespace LazyFormatterTest
{
  public class LazyFormatterTest
  {
    private static string payload = @"<genericRecord>
  <attrListMap>
    <attrList attrCode=""SSN"">
      <attr>
        <fieldMap>
          <field name=""idnumber"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{0}</value>
          </field>
        </fieldMap>

        <fieldMap>
          <field name=""idissuer"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{1}</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""PATNAME"">
      <attr>
        <fieldMap>
          <field name=""onmFirst"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{2}</value>
          </field>

          <field name=""onmLast"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{3}</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""BIRTHDT"">
      <attr>
        <fieldMap>
          <field name=""dateval"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">1912-02-14</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""HOMEPHON"">
      <attr>
        <fieldMap>
          <field name=""phnumber"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">101-123-7654</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
  </attrListMap>
</genericRecord>";

    private readonly static byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
    private MemoryStream targetStream;
    private Utf8LazyFormatter formatter;

    [GlobalSetup]
    public void GlobalSetup()
    {
      this.targetStream = new MemoryStream(payloadBytes.Length * 2);
      this.formatter = new Utf8LazyFormatter(
            payloadBytes, (byte)'{', (byte)'}'
        );
    }

    [Benchmark(Baseline = true)]
    public MemoryStream Benchmark_Formatter()
    {
      targetStream.Position = 0;
      formatter.Format(this.targetStream, 555667777, "SSA", "Mario", "Bros");
      return this.targetStream;
    }

    [Benchmark()]
    public string Benchmrk_StringInterpolation()
    {
      var idNumber = 555667777;
      var idIssuer = "SSA";
      var firstName = "Mario";
      var lastName = "Bros";

      var payload = @$"<genericRecord>
  <attrListMap>
    <attrList attrCode=""SSN"">
      <attr>
        <fieldMap>
          <field name=""idnumber"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{idNumber}</value>
          </field>
        </fieldMap>

        <fieldMap>
          <field name=""idissuer"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{idIssuer}</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""PATNAME"">
      <attr>
        <fieldMap>
          <field name=""onmFirst"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{firstName}</value>
          </field>

          <field name=""onmLast"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">{lastName}</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""BIRTHDT"">
      <attr>
        <fieldMap>
          <field name=""dateval"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">1912-02-14</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
    <attrList attrCode=""HOMEPHON"">
      <attr>
        <fieldMap>
          <field name=""phnumber"">
            <value xsi:type=""xs:string""
              xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema"">101-123-7654</value>
          </field>
        </fieldMap>
      </attr>
    </attrList>
  </attrListMap>
</genericRecord>";
      return payload;
    }
  }
}

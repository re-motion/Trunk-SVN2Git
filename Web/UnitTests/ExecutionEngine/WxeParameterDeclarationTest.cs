using System;
using System.Globalization;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeParameterDeclarationTest
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("p1", true, WxeParameterDirection.In, typeof (string)),
      new WxeParameterDeclaration ("p2", true, WxeParameterDirection.In, typeof (bool)),
      new WxeParameterDeclaration ("p3", true, WxeParameterDirection.In, typeof (DateTime)),
      new WxeParameterDeclaration ("p4", true, WxeParameterDirection.In, typeof (object))
  };

  [Test]
  public void TestParse1 ()
  {
    // "this \"special\", value", "true", "2004-03-25 12:00", var1
    string args = @"""this \""special\"", value"", ""true"", ""2004-03-25 12:00"", var1";
    object[] result = WxeFunction.ParseActualParameters (s_parameters, args, CultureInfo.InvariantCulture);
    Assert.AreEqual (4, result.Length);
    Assert.AreEqual ("this \"special\", value", result[0]);
    Assert.AreEqual (true, result[1]);
    Assert.AreEqual (new DateTime (2004, 3, 25, 12, 0, 0), result[2]);
    Assert.AreEqual (new WxeVariableReference ("var1"), result[3]);
  }

  [Test]
  public void TestParse2 ()
  {
    // "value", true, 2004-03-25 12:00, var1
    string args = @"""value"", true, 2004-03-25 12:00, var1";
    object[] result = WxeFunction.ParseActualParameters (s_parameters, args, CultureInfo.InvariantCulture);
    Assert.AreEqual (4, result.Length);
    Assert.AreEqual ("value", result[0]);
    Assert.AreEqual (true, result[1]);
    Assert.AreEqual (new DateTime (2004, 3, 25, 12, 0, 0), result[2]);
    Assert.AreEqual (new WxeVariableReference ("var1"), result[3]);
  }

  [Test]
  [ExpectedException (typeof (ApplicationException))]
  public void TestParseEx1 ()
  {
    WxeFunction.ParseActualParameters (s_parameters, "a, b\"b, c", CultureInfo.InvariantCulture);
  }

  [Test]
  [ExpectedException (typeof (ApplicationException))]
  public void TestParseEx2 ()
  {
    WxeFunction.ParseActualParameters (s_parameters, "a, \"xyz\"", CultureInfo.InvariantCulture);
  }

  public void g()
  {
    WxeParameterDeclaration parameter = 
        new WxeParameterDeclaration ("param", true, WxeParameterDirection.In, typeof (string));


  }
}

}

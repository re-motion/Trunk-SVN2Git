using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.Text.Diagnostic;

namespace Remotion.UnitTests.Text.Diagnostic
{

  [TestFixture]
  public class ToTest
  {

    [Test]
    [Ignore]
    public void ObjectTest ()
    {
      Object o = 5711;
      Assert.That (To.Text (o), Is.EqualTo(o.ToString()));

      Object o2 = new object();
      Assert.That (To.Text (o2), Is.EqualTo (o2.ToString ()));
    }

    [Test]
    public void FallbackToStringTest ()
    {
      FallbackToStringTestSingleType ("abcd EFGH");
      FallbackToStringTestSingleType (87971132);
      FallbackToStringTestSingleType (4786.5323);
      int i = 8723;
      FallbackToStringTestSingleType (i);
    }

    private void FallbackToStringTestSingleType<T> (T t)
    {
      Assert.That (To.Text (t), Is.EqualTo (t.ToString ()));
    }



    [Test]
    public void RegisteredHandlerTest ()
    {
      To.RegisterHandler<Object> (x => String.Format ("[Object: {0}]", x.ToString()));
      Object o = new object ();
      string toTextO = To.Text (o);
      Log ("toTextO=" + toTextO);
      Assert.That (toTextO, Is.EqualTo (String.Format ("[Object: {0}]", o.ToString ())));

      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1}]", x.Name, x.Int));
      var test = new To.Test ("That's not my name", 179);
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179]"));
    }

    public static void Log (string s)
    {
      Console.WriteLine (s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Console.WriteLine (String.Format(format),parameterArray);
    }

  }

}
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Text.Diagnostic;

namespace Remotion.UnitTests.Text.Diagnostic
{

  [TestFixture]
  public class ToTest
  {
    // TODO: Store & replace the ToTextProvider To.Text uses and restore it after the tests.

    public class Test
    {
      public Test ()
      {
        Name = "DefaultName";
        Int = 1234567;
      }

      public Test (string name, int i0)
      {
        Name = name;
        Int = i0;
        ListListString = new List<List<string>> ();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[,,] RectangularArray3D { get; set; }
    }

    public class Test2 
    {
      public Test2 ()
      {
        Name = "DefaultName";
        Int = 1234567;
      }

      public Test2 (string name, int i0)
      {
        Name = name;
        Int = i0;
        ListListString = new List<List<string>> ();
      }

      public string Name { get; set; }
      public int Int { get; set; }
      public LinkedList<string> LinkedListString { get; set; }
      public List<List<string>> ListListString { get; set; }
      public Object[][][] Array3D { get; set; }
      public Object[,] RectangularArray2D { get; set; }
      public Object[, ,] RectangularArray3D { get; set; }
    }


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
      //FallbackToStringTestSingleType ("abcd EFGH");
      FallbackToStringTestSingleType (87971132);
      //FallbackToStringTestSingleType (4786.5323);
      int i = 8723;
      FallbackToStringTestSingleType (i);
    }

    private void FallbackToStringTestSingleType<T> (T t)
    {
      Assert.That (To.Text (t), Is.EqualTo (t.ToString ()));
    }

    private void RegisterHandlers ()
    {
      To.RegisterHandler<Int32> ((x, ttb) => ttb.sb ("[Int32: ", "", ",", "", "]").ts(x).se());
      To.RegisterHandler<Test> ((x, ttb) => ttb.sb("<<Test: ","",";","",">>").m ("Name",x.Name).m("Int",x.Int).se());
    }


    [Test]
    //[Ignore]
    public void RegisteredHandlerTest ()
    {
      To.ClearHandlers ();
      RegisterHandlers();
      //int i = 34567; 
      //string toTextO = To.Text (i); 
      //Log ("toTextO=" + toTextO);
      //Assert.That (toTextO, Is.EqualTo (String.Format ("<<Object: {0}>>", i.ToString ())));

      var test = new ToTest.Test ("That's not my name", 179);
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("<<Test: Name=\"That's not my name\";Int=[Int32: 179]>>"));
    }

    [Test]
    public void NullTest ()
    {
      To.ClearHandlers ();
      Object o = null;
      Assert.That (To.Text (o), Is.EqualTo ("null"));
    }

    [Test]
    public void IntToStringFallbackTest ()
    {
      To.ClearHandlers ();
      int i = 908;
      Assert.That (To.Text (i), Is.EqualTo ("908"));
    }


    [Test]
    public void InitStandardHandlersTest ()
    {
      To.ClearHandlers ();
      To.TextEnableAutomatics (true);
      Assert.That (To.Text ("Some text"), Is.EqualTo ("\"Some text\""));
      Assert.That (To.Text ('x'), Is.EqualTo ("'x'"));
    }


    //[Test]
    //public void ClearHandlersTest ()
    //{
    //  To.RegisterHandler<Object> (x => "[ClearHandlersTest]" + x);
    //  Object o = new object ();
    //  string toTextTest = To.Text (o);
    //  Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
    //  Assert.That (To.Text (o), Is.EqualTo ("[ClearHandlersTest]" + o));
    //  To.ClearHandlers();
    //  Assert.That (To.Text (o), Is.EqualTo (o.ToString()));
    //}




    public static void Log (string s)
    {
      Console.WriteLine (s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }

  }

}
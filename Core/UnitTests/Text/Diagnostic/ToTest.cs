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

    private void RegisterHandlers ()
    {
      To.RegisterHandler<Object> (x => String.Format ("[Object: {0}]", x.ToString ()));
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1}]", x.Name, x.Int));
    }

    private void InitTestInstanceContainer (To.Test test)
    {
      test.ListListString.Add (new List<string> () { "Aaa","Bbb","Ccc","Ddd"});
      test.ListListString.Add (new List<string> () { "R1 C0", "R1 C1" });
      test.ListListString.Add (new List<string> () { "R2 C0", "R2 C1", "R2 C2","R2 C3","R2 C4" });
      test.ListListString.Add (new List<string> () { "R3 C0", "R3 C1", "R3 C2" });
    }

    [Test]
    public void RegisteredHandlerTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<Object> (x => String.Format ("[Object: {0}]", x.ToString ()));
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
    public void StringHandlerTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<String> (x => "\"" + x + "\"");
      string toTextTest = To.Text ("Some text");
      Log ("[StringHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("\"Some text\""));
    }

    [Test]
    public void CharHandlerTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<char> (x => "'" + x + "'");
      string toTextTest = To.Text ('x');
      Log ("[CharHandlerTest] toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("'x'"));
    }

    [Test]
    public void InitStandardHandlersTest ()
    {
      To.ClearHandlers ();
      To.RegisterStringHandlers ();
      Assert.That (To.Text ("Some text"), Is.EqualTo ("\"Some text\""));
      Assert.That (To.Text ('x'), Is.EqualTo ("'x'"));
    }


    [Test]
    public void ClearHandlersTest ()
    {
      To.RegisterHandler<Object> (x => "[ClearHandlersTest]" + x);
      Object o = new object ();
      string toTextTest = To.Text (o);
      Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
      Assert.That (To.Text (o), Is.EqualTo ("[ClearHandlersTest]" + o));
      To.ClearHandlers();
      Assert.That (To.Text (o), Is.EqualTo (o.ToString()));
    }

    [Test]
    public void StringNotEnumerableTest ()
    {
      To.ClearHandlers ();
      string s = "Wow look at the size of that thing";
      string toTextTest = To.Text (s);
      const string toTextTestExpected = "Wow look at the size of that thing";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }



    [Test]
    public void NullEnumerableTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), 
        To.Text (x.Int), To.Text (x.LinkedListString)));
      var test = new To.Test ("That's not my name", 179);
      test.LinkedListString = null;
      string toTextTest = To.Text (test);
      const string toTextTestExpected = "[Test: That's not my name;179;null]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void EmptyEnumerableTest ()
    {
      To.ClearHandlers ();
      //To.RegisterHandler<string> (x => x); // To prevent string being treated as collection
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.ListListString)));
      var test = new To.Test ("That's not my name", 179);
      test.LinkedListString = new LinkedList<string> ();
      string toTextTest = To.Text (test);
      const string toTextTestExpected = "[Test: That's not my name;179;{}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }

    [Test]
    public void OneDimensionalEnumerableTest ()
    {
      To.ClearHandlers ();
      //To.RegisterHandler<string> (x => x); // To prevent string being treated as collection
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.LinkedListString)));
      var test = new To.Test ("That's not my name", 179);
      string[] linkedListInit = { "A1", "A2", "A3", "A4", "A5" };
      test.LinkedListString = new LinkedList<string> (linkedListInit);
      //LogVariables ("LinkedListString.Count={0}", test.LinkedListString.Count);
      string toTextTest = To.Text (test);
      const string toTextTestExpected = "[Test: That's not my name;179;{A1,A2,A3,A4,A5}]";
      Assert.That (toTextTest, Is.EqualTo (toTextTestExpected));
    }


    [Test]
    public void TwoDimensionalEnumerableTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.ListListString)));
      var test = new To.Test ("That's not my name", 179);
      test.ListListString = new List<List<string>>() { new List<string>(){"A1","A2"} , new List<string>(){"B1","B2","B3" } };
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{A1,A2},{B1,B2,B3}}]"));
    }


    [Test]
    public void ThreeDimensionalEnumerableTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.Array3D)));
      var test = new To.Test ("That's not my name", 179);
      //Object[,] aaa = { { 1 } };
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91,82,73,64 } } };
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{{91,82,73,64}}}]"));
    }


    [Test]
    [Ignore] // TODO: Treat rectangular multidimensional arrays seperately from other collections
    public void RectangularArrayEnumerableTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<To.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.RectangularArray2D)));
      var test = new To.Test ("That's not my name", 179);
      //Object[,] aaa = { { 1 } };
      test.RectangularArray2D = new Object[,] {{1,3,5},{7,11,13}};
      //test.RectangularArray2D.
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{1,3,5},{7,11,13}}]"));
    }



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
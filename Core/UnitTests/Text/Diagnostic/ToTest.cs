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
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1}]", x.Name, x.Int));
    }

    private void InitTestInstanceContainer (ToTest.Test test)
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

      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1}]", x.Name, x.Int));
      var test = new ToTest.Test ("That's not my name", 179);
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
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), 
        To.Text (x.Int), To.Text (x.LinkedListString)));
      var test = new ToTest.Test ("That's not my name", 179);
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
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.ListListString)));
      var test = new ToTest.Test ("That's not my name", 179);
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
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.LinkedListString)));
      var test = new ToTest.Test ("That's not my name", 179);
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
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.ListListString)));
      var test = new ToTest.Test ("That's not my name", 179);
      test.ListListString = new List<List<string>>() { new List<string>(){"A1","A2"} , new List<string>(){"B1","B2","B3" } };
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{A1,A2},{B1,B2,B3}}]"));
    }


    [Test]
    public void ThreeDimensionalEnumerableTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.Array3D)));
      var test = new ToTest.Test ("That's not my name", 179);
      //Object[,] aaa = { { 1 } };
      test.Array3D = new Object[][][] { new Object[][] { new Object[] { 91,82,73,64 } } };
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{{91,82,73,64}}}]"));
    }


    [Test]
    public void RectangularArrayTest ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.RectangularArray2D)));
      var test = new ToTest.Test ("That's not my name", 179);
      test.RectangularArray2D = new Object[,] {{1,3,5},{7,11,13}};
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{1,3,5},{7,11,13}}]"));
    }

    [Test]
    public void RectangularArrayTest2 ()
    {
      To.ClearHandlers ();
      To.RegisterHandler<ToTest.Test> (x => String.Format ("[Test: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.RectangularArray3D)));
      To.RegisterHandler<ToTest.Test2> (x => String.Format ("[Test2: {0};{1};{2}]", To.Text (x.Name), To.Text (x.Int), To.Text (x.RectangularArray2D)));
      var test = new ToTest.Test ("That's not my name", 179);
      Test2[,,] at2 = new Test2[3, 3, 3];

      for (int i0 = 0; i0 < 2; i0++)
      {
        for (int i1 = 0; i1 < 2; i1++)
        {
          for (int i2 = 0; i2 < 2; i2++)
          {
            var test2 = new ToTest.Test2 (String.Format ("{0}-{1}-{2}", i0, i1, i2), i0 ^ i1 ^ i2);
            test2.RectangularArray2D = new string[,] { {"A" + i0,"B" + 1}, {"C" + i2,"D"} };
            at2[i0, i1, i2] = test2;
          }
        }
      }
      test.RectangularArray3D = new Object[,,]
                                {
                                    {
                                        { at2[0, 0, 0], at2[0, 0, 1] }, { at2[0, 1, 0], at2[0, 1, 1] },
                                    },
                                    {
                                        { at2[1, 0, 0], at2[1, 0, 1] }, { at2[1, 1, 0], at2[1, 1, 1] },
                                    },
                                };
      string toTextTest = To.Text (test);
      Log ("toTextTest=" + toTextTest);
      Assert.That (toTextTest, Is.EqualTo ("[Test: That's not my name;179;{{{[Test2: 0-0-0;0;{{A0,B1},{C0,D}}],[Test2: 0-0-1;1;{{A0,B1},{C1,D}}]},{[Test2: 0-1-0;1;{{A0,B1},{C0,D}}],[Test2: 0-1-1;0;{{A0,B1},{C1,D}}]}},{{[Test2: 1-0-0;1;{{A1,B1},{C0,D}}],[Test2: 1-0-1;0;{{A1,B1},{C1,D}}]},{[Test2: 1-1-0;0;{{A1,B1},{C0,D}}],[Test2: 1-1-1;1;{{A1,B1},{C1,D}}]}}}]"));
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
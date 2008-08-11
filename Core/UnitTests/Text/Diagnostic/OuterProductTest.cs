using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Text.Diagnostic;


namespace Remotion.UnitTests.Text.Diagnostic
{
  [TestFixture]
  public class OuterProductTest
  {
    //private class OuterProductVisitor : OuterProduct.IOuterProductVisitor { }

    [Test]
    public void NumberElementsPerDimensionCtorTest ()
    {
      int[] arrayDimensions = new int[] {5,7,11};
      var outerProduct = new OuterProduct (arrayDimensions);
      Assert.That (outerProduct.Length, Is.EqualTo (5*7*11)); 
    }

    [Test]
    public void ArrayCtorTest ()
    {
      String[,] rectangularArray = new string[,] {{"A1","A2"},{"B1","B2"},{"C1","C2"}};
      var outerProduct = new OuterProduct (rectangularArray);
      Assert.That (outerProduct.Length, Is.EqualTo (3*2));
    }

    [Test]
    public void ArrayToStringTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2" }, { "B1", "B2" } };
      var outerProduct = new OuterProduct (rectangularArray);
      string result = outerProduct.VisitedIndicesToString();
      Log (result);
      Assert.That (result, Is.EqualTo ("(0,0),(0,1),(1,0),(1,1)"));
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
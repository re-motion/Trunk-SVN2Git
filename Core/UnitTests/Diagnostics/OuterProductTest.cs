/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;


namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class OuterProductTest
  {
    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor implementations
    /// </summary>
    private abstract class OuterProductProcessorBase : Remotion.Diagnostics.OuterProductProcessorBase
    {
      protected Array _rectangularArray;
      protected StringBuilder _result = new StringBuilder();

      public OuterProductProcessorBase (Array rectangularArray)
      {
        _rectangularArray = rectangularArray;
      }

      public virtual String GetResult ()
      {
        return _result.ToString();
      }
    }


    private class OuterProductProcessorOneLineString : OuterProductProcessorBase
    {
      public OuterProductProcessorOneLineString (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          _result.Append (CollectionToSequenceString (ProcessingState.DimensionIndices));
        }
        else
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ","); // Seperator only if not first element
          _result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          _result.Append ("}");
        return true;
      }
    }


    private class OuterProductProcessorPrettyPrinter : OuterProductProcessorBase
    {
      public OuterProductProcessorPrettyPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      protected void AppendIndentedLine (string text)
      {
        _result.AppendLine();
        _result.Append (' ', ProcessingState.DimensionIndex);
        _result.Append (text);
      }

      protected virtual void AppendInnermostLoop ()
      {
        AppendIndentedLine (CollectionToSequenceString (ProcessingState.DimensionIndices));
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
          AppendInnermostLoop();
        else
          AppendIndentedLine ("{");
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          AppendIndentedLine ("}");
        return true;
      }
    }


    private class OuterProductProcessorArrayPrettyPrinter : OuterProductProcessorPrettyPrinter
    {
      public OuterProductProcessorArrayPrettyPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      protected override void AppendInnermostLoop ()
      {
        AppendIndentedLine (_rectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
      }
    }


    private class OuterProductProcessorArrayPrinter : OuterProductProcessorBase
    {
      public int NumberElementsToOutputInnermost { get; set; }
      public int NumberElementsToOutputAllButInnermost { get; set; }
      public int NumberElementsToOutputOverall { get; set; }

      public OuterProductProcessorArrayPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
        NumberElementsToOutputInnermost = -1;
        NumberElementsToOutputAllButInnermost = -1;
        NumberElementsToOutputOverall = -1;
      }

      protected virtual void AppendInnermostLoop ()
      {
        _result.Append (_rectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          if (NumberElementsToOutputInnermost > 0 && ProcessingState.ElementIndex >= NumberElementsToOutputInnermost)
          {
            _result.Append ("...");
            return false;
          }
          AppendInnermostLoop();
        }
        else
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ","); // Seperator only if not first element
          if (NumberElementsToOutputAllButInnermost > 0 && ProcessingState.ElementIndex >= NumberElementsToOutputAllButInnermost)
          {
            _result.Append ("...");
            return false;
          }
          _result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          _result.Append ("}");
        if (NumberElementsToOutputOverall > 0 && ProcessingState.NumberElementsProcessed >= NumberElementsToOutputOverall)
        {
          _result.Append (",...");
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor used in OuterProductIndexGenerator "pretty print rectangular arrays of arbitrary dimensions" code sample
    /// </summary>
    public class RectangularArrayToString : Remotion.Diagnostics.OuterProductProcessorBase
    {
      protected Array _rectangularArray;
      public readonly StringBuilder _result = new StringBuilder(); // To keep sample concise

      public RectangularArrayToString (Array rectangularArray)
      {
        _rectangularArray = rectangularArray;
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          _result.Append (_rectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
        }
        else
        {
          _result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          _result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          _result.Append ("}");
        return true;
      }
    }


    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor used in OuterProductIndexGenerator "create outer prodcut permutations" code sample
    /// </summary>
    public class OuterProductPermutations : Remotion.Diagnostics.OuterProductProcessorBase
    {
      public readonly List<int[]> outerProductPermutations = new List<int[]>(); // To keep sample concise

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          Log (CollectionToSequenceString (ProcessingState.DimensionIndices));
          outerProductPermutations.Add (ProcessingState.GetDimensionIndicesCopy());
        }
        return true;
      }
    }


    /// <summary>
    /// Helper function to convert a collection into a human-readable string; use To.Text-facility instead as soon as it is fully implemented.
    /// </summary>
    private static string CollectionToSequenceString (IEnumerable collection, string start, string seperator, string end)
    {
      var sb = new StringBuilder();

      sb.Append (start);
      bool insertSeperator = false; // no seperator before first element
      foreach (Object element in collection)
      {
        if (insertSeperator)
          sb.Append (seperator);
        else
          insertSeperator = true;

        sb.Append (element.ToString());
      }
      sb.Append (end);
      return sb.ToString();
    }


    private static string CollectionToSequenceString (IEnumerable collection)
    {
      return CollectionToSequenceString (collection, "(", ",", ")");
    }


    /// <summary>
    /// OuterProductIndexGenerator tests be here...
    /// </summary>
    [Test]
    public void NumberElementsPerDimensionCtorTest ()
    {
      int[] arrayDimensions = new int[] { 5, 7, 11 };
      var outerProduct = new OuterProductIndexGenerator (arrayDimensions);
      Assert.That (outerProduct.Length, Is.EqualTo (5*7*11));
    }


    [Test]
    public void ArrayCtorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2" }, { "B1", "B2" }, { "C1", "C2" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      Assert.That (outerProduct.Length, Is.EqualTo (3*2));
    }


    [Test]
    public void NestedForLoopsTest ()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append ("{");
      for (int i0 = 0; i0 < 2; ++i0)
      {
        sb.Append (i0 == 0 ? "" : ",");
        sb.Append ("{");
        for (int i1 = 0; i1 < 3; ++i1)
        {
          sb.Append (i1 != 0 ? "," : "");
          sb.Append ("(" + i0 + "," + i1 + ")");
        }
        sb.Append ("}");
      }
      sb.Append ("}");
      string s = sb.ToString();
      Log (s);
      Assert.That (s, Is.EqualTo ("{{(0,0),(0,1),(0,2)},{(1,0),(1,1),(1,2)}}"));
    }


    [Test]
    public void VisitorNestedForTest ()
    {
      String[,] rectangularArray = new string[,] { { null, null, null }, { null, null, null } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorOneLineString (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Log (s);
      Assert.That (s, Is.EqualTo ("{(0,0),(0,1),(0,2)},{(1,0),(1,1),(1,2)}"));
    }


    [Test]
    public void PermutationVisitorTest ()
    {
      var dimensionArray = new int[] { 2, 3, 2 };
      var outerProduct = new OuterProductIndexGenerator (dimensionArray);
      var processor = new OuterProductProcessorOneLineString (null);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Log (s);
      Assert.That (s, Is.EqualTo ("{{(0,0,0),(0,0,1)},{(0,1,0),(0,1,1)},{(0,2,0),(0,2,1)}},{{(1,0,0),(1,0,1)},{(1,1,0),(1,1,1)},{(1,2,0),(1,2,1)}}"));
    }


    [Test]
    public void VisitorNestedForTest2 ()
    {
      var rectangularArray = new string[,,] { { { null, null }, { null, null }, { null, null } }, { { null, null }, { null, null }, { null, null } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorOneLineString (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Log (s);
      Assert.That (s, Is.EqualTo ("{{(0,0,0),(0,0,1)},{(0,1,0),(0,1,1)},{(0,2,0),(0,2,1)}},{{(1,0,0),(1,0,1)},{(1,1,0),(1,1,1)},{(1,2,0),(1,2,1)}}"));
    }

    [Test]
    public void VisitorNestedForOuterProductProcessorPrettyPrinterTest ()
    {
      var rectangularArray = new string[,,] { { { null, null }, { null, null }, { null, null } }, { { null, null }, { null, null }, { null, null } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorPrettyPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Log (s);
      const string resultExpected =
          @"
{
 {
  (0,0,0)
  (0,0,1)
 }
 {
  (0,1,0)
  (0,1,1)
 }
 {
  (0,2,0)
  (0,2,1)
 }
}
{
 {
  (1,0,0)
  (1,0,1)
 }
 {
  (1,1,0)
  (1,1,1)
 }
 {
  (1,2,0)
  (1,2,1)
 }
}";
      Assert.That (s, Is.EqualTo (resultExpected));
    }


    [Test]
    public void VisitorNestedForOuterProductProcessorArrayPrettyPrinterTest ()
    {
      var rectangularArray = new string[,,] { { { "A0", "A1" }, { "B0", "B1" }, { "C0", "C1" } }, { { "D0", "D1" }, { "E0", "E1" }, { "F0", "F1" } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrettyPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Log (s);
      const string resultExpected =
          @"
{
 {
  A0
  A1
 }
 {
  B0
  B1
 }
 {
  C0
  C1
 }
}
{
 {
  D0
  D1
 }
 {
  E0
  E1
 }
 {
  F0
  F1
 }
}";
      Assert.That (s, Is.EqualTo (resultExpected));
    }


    [Test]
    public void RectangularArrayVisitorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Log (result);
      Assert.That (result, Is.EqualTo ("{A1,A2,A3},{B1,B2,B3},{C1,C2,C3}"));
    }

    [Test]
    public void RectangularArrayTerminatingVisitorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Log (result);
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},{C1,C2,...}"));
    }


    [Test]
    public void RectangularArrayTerminatingVisitorTest2 ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      processor.NumberElementsToOutputAllButInnermost = 3;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Log (result);
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},{C1,C2,...},..."));
    }

    [Test]
    public void RectangularArrayTerminatingVisitorTest3 ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      processor.NumberElementsToOutputAllButInnermost = 3;
      processor.NumberElementsToOutputOverall = 5;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Log (result);
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},..."));
    }


    [Test]
    public void ProcessSameOuterProductMultipleTimesTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      OuterProductProcessorArrayPrinter processor = null;
      int i = 0;
      for (; i < 3; ++i)
      {
        processor = new OuterProductProcessorArrayPrinter (rectangularArray);
        outerProduct.ProcessOuterProduct (processor);
      }
      Assert.That (i, Is.EqualTo (3));
      string result = processor.GetResult();
      Log (result);
      Assert.That (result, Is.EqualTo ("{A1,A2,A3},{B1,B2,B3},{C1,C2,C3},{D1,D2,D3}"));
    }


    [Test]
    public void SampleRectangularArrayToStringTest ()
    {
      List<String> resultStrings = new List<string> { "A1,A2,A3", "{A1,A2,A3},{B1,B2,B3},{C1,C2,C3}", "{{A1,A2},{B1,B2}},{{C1,C2},{D1,D2}}" };

      Array rectangularArray1D = new string[] { "A1", "A2", "A3" };
      Array rectangularArray2D = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      Array rectangularArray3D = new string[,,] { { { "A1", "A2" }, { "B1", "B2" } }, { { "C1", "C2" }, { "D1", "D2" } } };
      var arrays = new List<Array>() { rectangularArray1D, rectangularArray2D, rectangularArray3D };
      foreach (var array in arrays)
      {
        var outerProduct = new OuterProductIndexGenerator (array);
        var processor = new RectangularArrayToString (array);
        outerProduct.ProcessOuterProduct (processor);
        System.Console.WriteLine (processor._result.ToString());

        string result = processor._result.ToString();
        //Log (result);
        Assert.That (new List<String> { result }, Is.SubsetOf (resultStrings));
      }
    }


    [Test]
    public void SamplePermutationVisitorTest ()
    {
      var dimensionArray = new int[] { 2, 3, 2 };
      var outerProduct = new OuterProductIndexGenerator (dimensionArray);
      var processor = new OuterProductPermutations();
      outerProduct.ProcessOuterProduct (processor);
      var result = processor.outerProductPermutations;
      Log ("--------------------------");
      foreach (var ints in result)
      {
        System.Console.Write ("(");
        foreach (var i in ints)
          System.Console.Write (i + " ");
        System.Console.Write (") ");
        //Log (CollectionToSequenceString (ints));
      }
      var resultExpected = new int[][]
                           {
                               new int[] { 0, 0, 0 }, new int[] { 0, 0, 1 }, new int[] { 0, 1, 0 }, new int[] { 0, 1, 1 }, new int[] { 0, 2, 0 },
                               new int[] { 0, 2, 1 }, new int[] { 1, 0, 0 }, new int[] { 1, 0, 1 }, new int[] { 1, 1, 0 }, new int[] { 1, 1, 1 },
                               new int[] { 1, 2, 0 }, new int[] { 1, 2, 1 }
                           };
      Assert.That (result.ToArray(), Is.EqualTo (resultExpected));
    }


    /// <summary>
    /// Logging helper functions
    /// </summary>
    /// <param name="s"></param>
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
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("ScriptingPerformanceTests",
      new TypeLevelTypeFilter (new[] { typeof (Cascade) }));


    [Test]
    [Explicit]
    public void LongPropertyPathAccess_DlrVsClr ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  if cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == 'C0' :
    return cascade.Child.Child.Child.Child.Child.Child.Child.Name
  return 'FAILED'
";

      const string expressionScriptSourceCode =
"IIf( GLOBAL_cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == 'C0',GLOBAL_cascade.Child.Child.Child.Child.Child.Child.Child.Name,'FAILED')";


      var cascade = new Cascade (10);

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      privateScriptEnvironment.ImportHelperFunctions();
      privateScriptEnvironment.SetVariable ("GLOBAL_cascade", cascade);

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      var propertyPathAccessExpressionScript = new ExpressionScript<string> (
        _scriptContext, ScriptLanguageType.Python, expressionScriptSourceCode, privateScriptEnvironment
      );

      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000};
      ExecuteAndTime ("C# method", nrLoopsArray, delegate
      {
        if (cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == "C0")
        {
          return cascade.Child.Child.Child.Child.Child.Child.Child.Name;
        }
        return "FAILED";
      }
      );
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndTime ("expression script", nrLoopsArray, propertyPathAccessExpressionScript.Execute);
    }


    [Test]
    [Explicit]
    public void EmptyScriptCall ()
    {
      const string scriptFunctionSourceCode = @"
def Empty() :
  return None
";

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      var emptyScript = new ScriptFunction<Object> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "Empty"
      );

      var emptyExpression = new ExpressionScript<Object> (
        _scriptContext, ScriptLanguageType.Python,
        "None", privateScriptEnvironment
      );

      //var nrLoopsArray = new[] {1,1,10,100,1000,10000,100000,1000000};
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000};
      ExecuteAndTime ("empty script function", nrLoopsArray, emptyScript.Execute);
      ExecuteAndTime ("empty expression script", nrLoopsArray, emptyExpression.Execute);
      ExecuteAndTime ("empty expression script (uncompiled)", nrLoopsArray, emptyExpression.ExecuteUncompiled);
    }


    public void ExecuteAndTime (string testName, int nrLoops, Func<Object> func)
    {
      ExecuteAndTime (testName, new[] { nrLoops }, func);
    }

    public void ExecuteAndTime ( string testName, int[] nrLoopsArray, Func<Object> func)
    {
      object result = null;

      var timings = new System.Collections.Generic.List<long>();

      foreach (var nrLoops in nrLoopsArray)
      {
        System.GC.Collect (2);
        System.GC.WaitForPendingFinalizers ();

        Stopwatch stopwatch = new Stopwatch ();
        stopwatch.Start ();

        for (int i = 0; i < nrLoops; i++)
        {
          result = func ();
        }

        stopwatch.Stop ();
        timings.Add (stopwatch.ElapsedMilliseconds);
      }

      To.ConsoleLine.s ("Timings ").e (testName).s(",").e (() => nrLoopsArray).s (": ").nl ().sb ();
      foreach (var timing in timings)
      {
        To.Console.e (timing);
      }
      To.Console.se();
    }


    public void ExecuteAndTimeLongPropertyPathAccess (string testName, int nrLoops, Func<Cascade, string> func)
    {
      var cascade = new Cascade (10);
      object result = "xyz";

      System.GC.Collect (2);
      System.GC.WaitForPendingFinalizers ();

      Stopwatch stopwatch = new Stopwatch ();
      stopwatch.Start ();

      for (int i = 0; i < 1000000; i++)
      {
        result = func (cascade);
      }

      stopwatch.Stop ();
      double milliSeconds = stopwatch.ElapsedMilliseconds;
      Assert.That (result, Is.EqualTo ("C2"));
      To.ConsoleLine.e (() => result).e (() => milliSeconds);
    }


    private string TestMethod (Cascade c)
    {
      if (c.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == "C0")
      {
        return c.Child.Child.Child.Child.Child.Child.Child.Name;
      }
      return "FAILED";
    }
  }

  public class Cascade
  {
    public Cascade Child;
    public string Name;
    public Cascade (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new Cascade (nrChildren);
      }
    }
  }
}
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

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");


      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      ExecuteAndTime (TestMethod);
      ExecuteAndTime (  delegate(Cascade c)
      {
        if (c.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == "C0")
        {
          return c.Child.Child.Child.Child.Child.Child.Child.Name;
        }
        return "FAILED";
      }
      );
      ExecuteAndTime (c => propertyPathAccessScript.Execute (c));
    }

    public void ExecuteAndTime (Func<Cascade,string> func)
    {
      var cascade = new Cascade (10);
      string result = "xyz";

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
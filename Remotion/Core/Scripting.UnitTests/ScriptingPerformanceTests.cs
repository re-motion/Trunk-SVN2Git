// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create (
        "ScriptingPerformanceTests",
        new TypeLevelTypeFilter (new[] { typeof (Cascade), typeof (CascadeStableBinding) }));


    [Test]
    [Explicit]
    public void LongPropertyPathAccess_DlrVsClr ()
    {
      const string scriptFunctionSourceCode =
          @"
import clr
def PropertyPathAccess(cascade) :
  if cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == 'C0' :
    return cascade.Child.Child.Child.Child.Child.Child.Child.Name
  return 'FAILED'
";

      const string expressionScriptSourceCode =
          "IIf( GLOBAL_cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == 'C0',GLOBAL_cascade.Child.Child.Child.Child.Child.Child.Child.Name,'FAILED')";


      var cascade = new Cascade (10);

      var privateScriptEnvironment = ScriptEnvironment.Create();
      privateScriptEnvironment.ImportIifHelperFunctions();
      privateScriptEnvironment.SetVariable ("GLOBAL_cascade", cascade);

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );

      var propertyPathAccessExpressionScript = new ExpressionScript<string> (
          _scriptContext, ScriptLanguageType.Python, expressionScriptSourceCode, privateScriptEnvironment
          );

      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000};
      ExecuteAndLogTimings (
          "C# method",
          nrLoopsArray,
          delegate
          {
            if (cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == "C0")
              return cascade.Child.Child.Child.Child.Child.Child.Child.Name;
            return "FAILED";
          }
          );
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings ("expression script", nrLoopsArray, propertyPathAccessExpressionScript.Execute);
    }


    [Test]
    [Explicit]
    public void EmptyScriptCall ()
    {
      const string scriptFunctionSourceCode = @"
def Empty() :
  return None
";

      var privateScriptEnvironment = ScriptEnvironment.Create();

      var emptyScript = new ScriptFunction<Object> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "Empty"
          );

      var emptyExpression = new ExpressionScript<Object> (
          _scriptContext,
          ScriptLanguageType.Python,
          "None",
          privateScriptEnvironment
          );

      //var nrLoopsArray = new[] {1,1,10,100,1000,10000,100000,1000000};
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      ExecuteAndLogTimings ("empty script function", nrLoopsArray, emptyScript.Execute);
      ExecuteAndLogTimings ("empty expression script", nrLoopsArray, emptyExpression.Execute);
      ExecuteAndLogTimings ("empty expression script (uncompiled)", nrLoopsArray, emptyExpression.ExecuteUncompiled);
    }


    public void ExecuteAndLogTimings (string testName, int nrLoops, Func<Object> func)
    {
      ExecuteAndLogTimings (testName, new[] { nrLoops }, func);
    }

    public void ExecuteAndLogTimings (string testName, int[] nrLoopsArray, Func<Object> func)
    {
      object result = null;

      var timings = new System.Collections.Generic.List<long>();

      foreach (var nrLoops in nrLoopsArray)
      {
        System.GC.Collect (2);
        System.GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < nrLoops; i++)
          result = func();

        stopwatch.Stop();
        timings.Add (stopwatch.ElapsedMilliseconds);
      }

      To.ConsoleLine.s ("Timings ").e (testName).s (",").e (() => nrLoopsArray).s (": ").nl().sb();
      foreach (var timing in timings)
        To.Console.e (timing);
      To.Console.se();
    }


    [Test]
    [Explicit]
    public void LongPropertyPathAccess_StableBindingSimple ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.GetChild().GetChild().GetName()
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);
      //var cascadeStableBinding = ObjectFactory.Create<CascadeStableBinding> (ParamList.Create (numberChildren));
      var cascadeLocalStableBinding = new CascadeLocalStableBinding (numberChildren);

      var cascadeGetCustomMemberReturnsAttributeProxyFromMap = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (numberChildren);
      cascadeGetCustomMemberReturnsAttributeProxyFromMap.AddAttributeProxy ("GetChild", cascade, _scriptContext);
      cascadeGetCustomMemberReturnsAttributeProxyFromMap.AddAttributeProxy ("GetName", cascade, _scriptContext);


      var privateScriptEnvironment = ScriptEnvironment.Create();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );


      privateScriptEnvironment.ImportIifHelperFunctions();
      privateScriptEnvironment.SetVariable ("GLOBAL_cascade", cascade);
      var expression = new ExpressionScript<Object> (
          _scriptContext,
          ScriptLanguageType.Python,
          "GLOBAL_cascade.GetChild().GetChild().GetName()",
          privateScriptEnvironment
          );


      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
      ExecuteAndLogTimings (
          "script function (local stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeLocalStableBinding));
      ExecuteAndLogTimings (
          "script function (from map)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsAttributeProxyFromMap));
    }


    [Test]
    [Explicit]
    public void LongPropertyPathAccess_StableBinding ()
    {
      const string scriptFunctionSourceCode =
          @"
import clr
def PropertyPathAccess(cascade) :
  if cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName() == 'C0' :
    return cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName()
  return 'FAILED'
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);
      //var cascadeStableBinding = ObjectFactory.Create<CascadeStableBinding> (ParamList.Create (numberChildren));

      var privateScriptEnvironment = ScriptEnvironment.Create();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
    }


    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Name
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);
      //var cascadeStableBinding = ObjectFactory.Create<CascadeStableBinding> (ParamList.Create (numberChildren));
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);
      var cascadeGetCustomMember = new CascadeGetCustomMemberReturnsString (numberChildren);

      var attributeNameProxy = _scriptContext.GetAttributeProxy (cascade, "Name");
      var cascadeGetCustomMemberReturnsFixedAttributeProxy = new CascadeGetCustomMemberReturnsFixedAttributeProxy (numberChildren, attributeNameProxy);


      var privateScriptEnvironment = ScriptEnvironment.Create();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
      ExecuteAndLogTimings ("script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMember));
      ExecuteAndLogTimings (
          "script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsFixedAttributeProxy));
    }


    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember_FixedAttributeProxy ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.GetName()
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);

      var attributeNameProxy = _scriptContext.GetAttributeProxy (cascade, "GetName");
      var cascadeGetCustomMemberReturnsFixedAttributeProxy = new CascadeGetCustomMemberReturnsFixedAttributeProxy (numberChildren, attributeNameProxy);

      var privateScriptEnvironment = ScriptEnvironment.Create();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings (
          "script function (FixedAttributeProxy)",
          nrLoopsArray,
          () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsFixedAttributeProxy));
    }

    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember_AttributeProxyFromMap ()
    {
      const string scriptFunctionSourceCode =
          @"
import clr
def PropertyPathAccess(cascade) :
  if cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName() == 'C0' :
    return cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName()
  return 'FAILED'
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);

      var cascadeGetCustomMemberReturnsAttributeProxyFromMap = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (numberChildren);
      cascadeGetCustomMemberReturnsAttributeProxyFromMap.AddAttributeProxy ("GetName", cascade, _scriptContext);
      cascadeGetCustomMemberReturnsAttributeProxyFromMap.AddAttributeProxy ("GetChild", cascade, _scriptContext);

      var privateScriptEnvironment = ScriptEnvironment.Create();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
          _scriptContext,
          ScriptLanguageType.Python,
          scriptFunctionSourceCode,
          privateScriptEnvironment,
          "PropertyPathAccess"
          );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      var nrLoopsArray = new[] { 10 };
      ExecuteAndLogTimings ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndLogTimings (
          "script function (AttributeProxyFromMap)",
          nrLoopsArray,
          () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsAttributeProxyFromMap));
    }


    [Test]
    [Explicit]
    public void GetAttributeProxyChainCallPerformance ()
    {
      // ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);

      var proxied0 = new Cascade (10);

      _scriptContext.Execute (delegate
      {
        var currentStableBindingProxyProvider = ScriptContext.Current.StableBindingProxyProvider;

        var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
        const string attributeName = "GetName";
        ScriptingHelper.ExecuteAndTime ("Direct", nrLoopsArray, () => currentStableBindingProxyProvider.GetAttributeProxy (proxied0, attributeName));
        ScriptingHelper.ExecuteAndTime (
            "Indirect", nrLoopsArray, () => ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied0, attributeName));

        return 0;
      });
    }


    public void ExecuteAndTimeLongPropertyPathAccess (string testName, int nrLoops, Func<Cascade, string> func)
    {
      var cascade = new Cascade (10);
      object result = "xyz";

      System.GC.Collect (2);
      System.GC.WaitForPendingFinalizers();

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < 1000000; i++)
        result = func (cascade);

      stopwatch.Stop();
      double milliSeconds = stopwatch.ElapsedMilliseconds;
      Assert.That (result, Is.EqualTo ("C2"));
      To.ConsoleLine.e (() => result).e (() => milliSeconds);
    }
  }
}
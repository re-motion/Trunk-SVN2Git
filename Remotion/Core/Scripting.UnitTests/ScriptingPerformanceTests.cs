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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Diagnostics.ToText;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("ScriptingPerformanceTests",
      new TypeLevelTypeFilter (new[] { typeof (Cascade), typeof(CascadeStableBinding) }));


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

 
      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );


      privateScriptEnvironment.ImportHelperFunctions ();
      privateScriptEnvironment.SetVariable ("GLOBAL_cascade", cascade);
      var expression = new ExpressionScript<Object> (
        _scriptContext, ScriptLanguageType.Python,
        "GLOBAL_cascade.GetChild().GetChild().GetName()", privateScriptEnvironment
      );


      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndTime ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
      ExecuteAndTime ("script function (local stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeLocalStableBinding));
      ExecuteAndTime ("script function (from map)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsAttributeProxyFromMap));
      //ExecuteAndTime ("uncompiled expression", nrLoopsArray, () => expression.Execute ());
      //ExecuteAndTime ("uncompiled expression", nrLoopsArray, () => expression.ExecuteUncompiled ());
    }


    [Test]
    [Explicit]
    public void LongPropertyPathAccess_StableBinding ()
    {
      const string scriptFunctionSourceCode = @"
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

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000};
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndTime ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
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
      var cascadeStableBinding = new CascadeStableBinding(numberChildren);
      var cascadeGetCustomMember = new CascadeGetCustomMemberReturnsString (numberChildren);

      ScriptContext.SwitchAndHoldScriptContext (_scriptContext);
      var attributeNameProxy = ScriptContext.GetAttributeProxy (cascade, "Name");
      ScriptContext.ReleaseScriptContext (_scriptContext);
      var cascadeGetCustomMemberReturnsFixedAttributeProxy = new CascadeGetCustomMemberReturnsFixedAttributeProxy (numberChildren, attributeNameProxy);
     


      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      //ExecuteAndTime ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
      ExecuteAndTime ("script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMember));
      ExecuteAndTime ("script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsFixedAttributeProxy));
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

      ScriptContext.SwitchAndHoldScriptContext (_scriptContext);
      var attributeNameProxy = ScriptContext.GetAttributeProxy (cascade, "GetName");
      ScriptContext.ReleaseScriptContext (_scriptContext);
      var cascadeGetCustomMemberReturnsFixedAttributeProxy = new CascadeGetCustomMemberReturnsFixedAttributeProxy (numberChildren, attributeNameProxy);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndTime ("script function (FixedAttributeProxy)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsFixedAttributeProxy));
    }

    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember_AttributeProxyFromMap ()
    {
      const string scriptFunctionSourceCode = @"
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

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000, 1000000 };
      //var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000, 100000 };
      var nrLoopsArray = new[] { 10 };
      ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ExecuteAndTime ("script function (AttributeProxyFromMap)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsAttributeProxyFromMap));
    }


    [Test]
    [Explicit]
    public void CascadeStableBinding ()
    {
//      const string scriptFunctionSourceCode = @"
//import clr
//def PropertyPathAccess(cascade) :
//  if cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name == 'C0' :
//    return cascade.Child.Child.Child.Child.Child.Child.Child.Name
//  return 'FAILED'
//";

      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  if cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName() == 'C0' :
    return cascade.GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetChild().GetName()
  return 'FAILED'
";


      const int numberChildren = 10;
      var cascadeStableBinding = ObjectFactory.Create<CascadeStableBinding> (ParamList.Create (numberChildren));

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var scriptContext = ScriptContext.Create ("CascadeStableBinding",
          //new TypeLevelTypeFilter (new[] { typeof (Cascade), typeof (CascadeStableBinding) }));
          new TypeLevelTypeFilter (new[] { typeof (CascadeStableBinding) }));

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      propertyPathAccessScript.Execute (cascadeStableBinding);
    }



    [Test]
    [Explicit]
    public void GetAttributeProxyChainCallPerformance ()
    {
      // ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);

      var proxied0 = new Cascade (10);

      ScriptContext.SwitchAndHoldScriptContext (_scriptContext);
      var currentStableBindingProxyProvider = ScriptContext.Current.StableBindingProxyProvider;

      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      const string attributeName = "GetName";
      ScriptingHelper.ExecuteAndTime ("Direct", nrLoopsArray, () => currentStableBindingProxyProvider.GetAttributeProxy (proxied0, attributeName));
      ScriptingHelper.ExecuteAndTime ("Indirect", nrLoopsArray, () => ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied0, attributeName));

      ScriptContext.ReleaseScriptContext (_scriptContext);
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

    public Cascade ()
    {
      
    }

    public Cascade (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new Cascade (nrChildren);
      }
    }

    public Cascade GetChild ()
    {
      return Child;
    }

    public string GetName ()
    {
      return Name;
    }
  }

  //[Uses (typeof (StableBindingMixin))]
  public class CascadeStableBinding : Cascade
  {
    public CascadeStableBinding (int nrChildren) 
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeStableBinding (nrChildren);
      }
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return ScriptContext.GetAttributeProxy (this, name);
    }
  }


  public class CascadeLocalStableBinding : Cascade
  {
    private StableBindingProxyProvider _proxyProvider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (CascadeLocalStableBinding) }), ScriptingHelper.CreateModuleScope ("CascadeLocalStableBinding"));

    public CascadeLocalStableBinding (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeLocalStableBinding (nrChildren);
      }
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return _proxyProvider.GetAttributeProxy (this, name);
    }
  }

 

  public class CascadeGetCustomMemberReturnsString : Cascade
  {
    public CascadeGetCustomMemberReturnsString (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeGetCustomMemberReturnsString (nrChildren);
      }
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      if (name == "Name")
      {
        return GetName ();
      }
      else
      {
        throw new NotSupportedException (String.Format ("Attribute {0} not supported by CascadeGetCustomMemberReturnsString", name));
      }
    }
  }


  public class CascadeGetCustomMemberReturnsFixedAttributeProxy : Cascade
  {
    private readonly object _attributeProxy;

    public CascadeGetCustomMemberReturnsFixedAttributeProxy (int nrChildren, object attributeProxy)
    {
      _attributeProxy = attributeProxy;
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeGetCustomMemberReturnsFixedAttributeProxy (nrChildren, attributeProxy);
      }
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return _attributeProxy;
    }
  }


  //public class CascadeGetCustomMemberReturnsAttributeProxyFromMap : Cascade
  //{
  //  private readonly Dictionary<string, object> _attributeProxyMap = new Dictionary<string, object>();

  //  public CascadeGetCustomMemberReturnsAttributeProxyFromMap (int nrChildren)
  //  {
  //    --nrChildren;
  //    Name = "C" + nrChildren;
  //    if (nrChildren > 0)
  //    {
  //      Child = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (nrChildren);
  //    }
  //  }

  //  public void AddAttributeProxy(string name, object proxied, ScriptContext scriptContext)
  //  {
  //    ScriptContext.SwitchAndHoldScriptContext (scriptContext);
  //    var attributeNameProxy = ScriptContext.GetAttributeProxy (proxied, name);
  //    _attributeProxyMap[name] = attributeNameProxy;
  //    ScriptContext.ReleaseScriptContext (scriptContext);
  //  }

  //  [SpecialName]
  //  public object GetCustomMember (string name)
  //  {
  //    //To.ConsoleLine.s ("CascadeGetCustomMemberReturnsAttributeProxyFromMap.GetCustomMember").e(() => name);
  //    return _attributeProxyMap[name];
  //  }
  //}


  public class CascadeGetCustomMemberReturnsAttributeProxyFromMap : Cascade
  {
    //private readonly Dictionary<string, object> _attributeProxyMap = new Dictionary<string, object> ();
    protected readonly Dictionary<Tuple<Type, string>, object> _attributeProxyMap = new Dictionary<Tuple<Type, string>, object> ();

    public CascadeGetCustomMemberReturnsAttributeProxyFromMap (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (nrChildren);
      }
    }

    public void AddAttributeProxy (string name, object proxied, ScriptContext scriptContext)
    {
      var type = this.GetType ();
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      var attributeNameProxy = ScriptContext.GetAttributeProxy (proxied, name);
      _attributeProxyMap[new Tuple<Type, string> (type,name)] = attributeNameProxy;
      ScriptContext.ReleaseScriptContext (scriptContext);
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      //To.ConsoleLine.s ("CascadeGetCustomMemberReturnsAttributeProxyFromMap.GetCustomMember").e(() => name);
      var type = this.GetType();
      return _attributeProxyMap[new Tuple<Type, string> (type, name)];
    }
  }


}
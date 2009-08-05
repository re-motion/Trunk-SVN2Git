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
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyProviderPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("rubicon.eu.Remotion.Scripting.StableBindingProxyProviderPerformanceTests",
      new TypeLevelTypeFilter (new[] { typeof (ICascade1) }));



    [Test]
    public void SimplePropertyAccess_GetCustomMember1 ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
";

      const int numberChildren = 10;
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      var nrLoopsArray = new[] { 1, 1, 100 };
      var timingStableBinding = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding))[2];

      To.ConsoleLine.e (() => timingStableBinding);
    }


    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember2 ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);
      var cascadeStableBindingFromMixin = ObjectFactory.Create<CascadeStableBindingFromMixin> (ParamList.Create (numberChildren));

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests", "Cascade");

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      var nrLoopsArray = new[] { 1, 1, 100 };
      var timing = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascade))[2];
      var timingStableBinding = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding))[2];
      var timingStableBindingFromMixin = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBindingFromMixin))[2];

      To.ConsoleLine.e (() => timing).e (() => timingStableBinding).e (() => timingStableBindingFromMixin);
    }


    public interface ICascade1
    {
      Cascade Child { get; set; }
      string Name { get; set; }
    }

    public interface ICascade2
    {
      //Cascade Child { get; set; }
      string Name { get; set; }
    }

    public class Cascade : ICascade1
    {
      protected Cascade _child;
      protected string _name;

      public Cascade ()
      {

      }

      public Cascade (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
        if (nrChildren > 0)
        {
          Child = new Cascade (nrChildren);
        }
      }

      public Cascade Child
      {
        get { return _child; }
        set { _child = value; }
      }

      string ICascade1.Name
      {
        get { return "ICascade1.Name"; }
        set { _name = value + "-ICascade1.Name"; }
      }

      public Cascade GetChild ()
      {
        return Child;
      }

      public string GetName ()
      {
        return _name;
      }
    }


     public class CascadeAmbigous : Cascade, ICascade2
    {
      string ICascade2.Name
      {
        get { return "ICascade2.Name"; }
        set { _name = value + "-ICascade2.Name"; }
      }
    }


    public class CascadeStableBinding : CascadeAmbigous
    {
      public CascadeStableBinding (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
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


    [Uses (typeof (StableBindingMixin))]
    public class CascadeStableBindingFromMixin : CascadeAmbigous
    {
      public CascadeStableBindingFromMixin (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
        if (nrChildren > 0)
        {
          Child = new CascadeStableBindingFromMixin (nrChildren);
        }
      }
    }

  }
}
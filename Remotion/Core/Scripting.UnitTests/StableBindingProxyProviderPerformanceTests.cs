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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyProviderPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("rubicon.eu.Remtoion.Scripting.StableBindingProxyProviderPerformanceTests",
      new TypeLevelTypeFilter (new[] { typeof (Cascade), typeof (CascadeStableBinding) }));


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
      ScriptingHelper.ExecuteAndTime ("script function", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ScriptingHelper.ExecuteAndTime ("script function (stable binding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
      ScriptingHelper.ExecuteAndTime ("script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMember));
      ScriptingHelper.ExecuteAndTime ("script function (GetCustomMember)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeGetCustomMemberReturnsFixedAttributeProxy));
    }
  }
}
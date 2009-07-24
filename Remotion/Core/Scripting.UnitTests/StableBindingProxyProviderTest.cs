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
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyProviderTest
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("Remotion.Scripting.StableBindingProxyProviderTest",
      new TypeLevelTypeFilter (new[] { typeof (ProxiedChildChildChild) }));

    [Test]
    [Ignore]
    public void GetTypeMemberProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), ScriptingHelper.CreateModuleScope ("GetTypeMemberProxy"));

      var proxied = new ProxiedChildChildChild("abrakadava");
      const string attributeName = "PrependName";

      var typeMemberProxy = provider.GetTypeMemberProxy (proxied.GetType(), attributeName);

      const string scriptFunctionSourceCode = @"
def TestTypeMemberProxy(typeMemberProxy) :
  return typeMemberProxy.PrependName(' simsalbum',2)
";

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      var testTypeMemberProxyScript = new ScriptFunction<Object, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "TestTypeMemberProxy");

      var result = testTypeMemberProxyScript.Execute (typeMemberProxy);
      Assert.That (result, Is.EqualTo ("ProxiedChildChildChild "));

    }
  }
}
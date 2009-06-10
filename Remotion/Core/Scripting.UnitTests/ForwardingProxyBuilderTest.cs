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
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ForwardingProxyBuilderTest
  {
    private ModuleScope _moduleScope;


    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      if (_moduleScope != null)
      {
        if (_moduleScope.StrongNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (true);
        if (_moduleScope.WeakNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (false);
      }
    }

    public ModuleScope ModuleScope
    {
      get
      {
        if (_moduleScope == null)
        {
          const string name = "Remotion.Scripting.CodeGeneration.Generated.ForwardingProxyBuilderTest";
          const string nameSigned = name + ".Signed";
          const string nameUnsigned = name + ".Unsigned";
          const string ext = ".dll";
          _moduleScope = new ModuleScope (true, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
        }
        return _moduleScope;
      }
    }


    [Test]
    public void BuildProxyType ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("ForwardingProxyBuilder_BuildProxyTypeTest", ModuleScope, typeof (Proxied), new Type[0]);
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      FieldInfo proxiedFieldInfo = proxy.GetType ().GetField ("_proxied", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (proxiedFieldInfo.GetValue (proxy), Is.EqualTo (proxied));
      Assert.That (proxiedFieldInfo.IsInitOnly, Is.True);
      Assert.That (proxiedFieldInfo.IsPrivate, Is.True);
    }


    private void SaveAndVerifyModuleScopeAssembly (bool strongNamed)
    {
      var path = _moduleScope.SaveAssembly (strongNamed);
      PEVerifier.VerifyPEFile (path);
    }
  }
}
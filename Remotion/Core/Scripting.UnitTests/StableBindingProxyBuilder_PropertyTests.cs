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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilder_PropertyTests : StableBindingProxyBuilderTest
  {
    [Test]
    [Explicit]
    public void BuildProxyType_Property ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IAmbigous2), typeof (IAmbigous1) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_Property"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      //var knownBaseTypeMethods = knownBaseTypes.SelectMany (t => t.GetMethods ()).Where (m => m.IsSpecialName == false);
      //var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object));
      //var interfaceMethods = knownInterfaceTypes.SelectMany (t => t.GetMethods (BindingFlags.Instance | BindingFlags.Public));

      ////To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (interfaceMethods).nl (3).e (proxyMethods).nl (3);

      //Assert.That (knownBaseTypeMethods.Count () + interfaceMethods.Count (), Is.EqualTo (proxyMethods.Count ()));




      // Adding IAmbigous2 interface adds StringTimes(string,int) explicit interface implementation
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous1), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous2), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
    }



  }
}
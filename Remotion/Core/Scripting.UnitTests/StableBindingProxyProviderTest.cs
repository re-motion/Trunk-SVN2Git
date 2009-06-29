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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyProviderTest
  {
    [Test]
    public void Ctor ()
    {
      var typeArbiter = new TypeLevelTypeArbiter(new Type[0]);
      var proxiedType = typeof (string);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter);
      Assert.That (stableBindingProxyBuilder.ProxiedType, Is.EqualTo (proxiedType));
      Assert.That (stableBindingProxyBuilder.GetTypeArbiter (), Is.EqualTo (typeArbiter));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap ()
    {
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { typeIAmbigous2, typeof(Proxied) });
      // Note: ProxiedChild implements IAmbigous1 and IAmbigous2
      var proxiedType = typeof(ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter);

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.BuildClassMethodToInterfaceMethodsMap();

      //To.ConsoleLine.e (classMethodToInterfaceMethodsMap);

      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod(proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");

      //To.ConsoleLine.e (stringTimesMethod).nl ().e (stringTimesIAmbigous2InterfaceMethod);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (1));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous2InterfaceMethod].ToList(), Is.EquivalentTo (ListMother.New(stringTimesIAmbigous2ClassMethod)));
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
  }
}
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class MethodInfoEqualityComparerTest
  {
    [Test]
    public void Equals_MethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (Proxied), "Sum");
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "Sum");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals(methodFromBaseType,method), Is.True);
    }

    [Test]
    public void Equals_InterfaceMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (Proxied), "GetName");
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "GetName");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_DifferingArgumentsMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (ProxiedChild), "BraKet", new []  {typeof(string), typeof(int)});
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "BraKet", new Type[0]);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }
  }
}
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

    // Comparing MethodInfo between method and method hiding method through "new" works.
    [Test]
    public void Equals_HiddenMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (Proxied), "PrependName", new[] { typeof(string)});
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "PrependName", new[] { typeof(string)});
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
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


    [Test]
    public void Equals_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (Proxied), "GenericToString");
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "GenericToString");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 2);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_Differing_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }

    [Test]
    public void Equals_GenericMethodFromBaseType_NonGenericOverloadInType ()
    {
      var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", new[] { typeof (int), typeof (int) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }

    //[Test]
    //public void Equals_GenericClass_GenericMethodFromBaseType ()
    //{
    //  var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
    //  var method = GetAnyGenericInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
    //  Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    //}

    [Test]
    public void Equals_GenericClass_MethodWithGenericClassArgumentsFromBaseType ()
    {
      var methodFromBaseType = GetAnyInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      var method = GetAnyInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_GenericClass_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      var method = GetAnyGenericInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    [Explicit]
    public void Equals_GenericClass_GenericMethodFromBaseType2 ()
    {
      var methodFromBaseType = GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
      var methods = GetAnyGenericInstanceMethodArray (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
      Assert.That (methods.Length, Is.EqualTo (2));

      //To.ToTextProvider.Settings.UseAutomaticObjectToText = true;
      To.ConsoleLine.e (methods[0].Name).nl ().e (methods[1].Name).nl ().e (methodFromBaseType.Name);
      To.ConsoleLine.e (methods[0].ReturnType).nl ().e (methods[1].ReturnType).nl ().e (methodFromBaseType.ReturnType);
      To.ConsoleLine.e (methods[0].Attributes).nl ().e (methods[1].Attributes).nl ().e (methodFromBaseType.Attributes);
      To.ConsoleLine.e (methods[0].GetParameters ().Select (pi => pi.Attributes)).nl ().e (methods[1].GetParameters ().Select (pi => pi.Attributes)).nl ().e (methodFromBaseType.GetParameters ().Select (pi => pi.Attributes));
      To.ConsoleLine.e (methods[0].GetParameters ().Select (pi => pi.ParameterType)).nl ().e (methods[1].GetParameters ().Select (pi => pi.ParameterType)).nl ().e (methodFromBaseType.GetParameters ().Select (pi => pi.ParameterType));
      //To.ToTextProvider.Settings.UseAutomaticObjectToText = false;

      var a0 = methods[0].GetParameters()[2];
      var a1 = methods[1].GetParameters ()[2];
      var ax = methodFromBaseType.GetParameters ()[2];

      var x = methods[0].GetParameters()[2];

      Assert.That (methodFromBaseType, Is.Not.EqualTo (methods[1]));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[1]), Is.True);
      //Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[0]), Is.True);
    }


    private MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }

    private MethodInfo GetAnyGenericInstanceMethod (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).Single();
    }

    private MethodInfo[] GetAnyGenericInstanceMethodArray (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).ToArray();
    }
  }
}
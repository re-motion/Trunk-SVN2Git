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
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (Proxied), "Sum");
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "Sum");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals(methodFromBaseType,method), Is.True);
    }


    public class CtorTest
    {
      public string Foo (string text)
      {
        return "CtorTest " + text;
      }
    }

    public interface ICtorTestFoo
    {
      string Foo (string text);
    }

    public class CtorTestChild : CtorTest, ICtorTestFoo
    {
      public new string Foo (string text)
      {
        return "CtorTestChild " + text;
      }
    }

    [Test]
    public void Ctor_Mask ()
    {
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (CtorTest), "Foo", new[] { typeof (string) });
      var childMethod = ScriptingHelper.GetAnyInstanceMethod (typeof (CtorTestChild), "Foo", new[] { typeof (string) });

      Assert.That (method.Attributes, Is.EqualTo (MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.HideBySig));
      const MethodAttributes childMethodAdditionalAttributes = MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.VtableLayoutMask;
      Assert.That (childMethod.Attributes, Is.EqualTo (method.Attributes | childMethodAdditionalAttributes));

      var comparerDefault = new MethodInfoEqualityComparer ();
      Assert.That (comparerDefault.Equals (method, childMethod), Is.False);

      var comparerNoVirtualNoFinal = new MethodInfoEqualityComparer (~childMethodAdditionalAttributes);
      Assert.That (comparerNoVirtualNoFinal.Equals (method, childMethod), Is.True);

      var comparerMethodFromBaseTypeAttributes = new MethodInfoEqualityComparer (method.Attributes);
      Assert.That (comparerMethodFromBaseTypeAttributes.Equals (method, childMethod), Is.True);
    }


    // Comparing MethodInfo between method and method hiding method through "new" works.
    [Test]
    public void Equals_MethodFromBaseTypeHiddenByInterfaceMethod ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (Proxied), "PrependName", new[] { typeof (string) });
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "PrependName", new[] { typeof (string) });
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));

      // Default comparison taking all method attributes into account fails, since IPrependName adds the Final, Virtual and 
      // VtableLayoutMask attributes.
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
      
      // Comparing ignoring method attributes coming from IPrependName works.
      var comparerNoVirtualNoFinal = new MethodInfoEqualityComparer (~(MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.VtableLayoutMask));
      Assert.That (comparerNoVirtualNoFinal.Equals (methodFromBaseType, method), Is.True);
    }


    [Test]
    public void Equals_InterfaceMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (Proxied), "GetName");
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "GetName");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_DifferingArgumentsMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChild), "BraKet", new[] { typeof (string), typeof (int) });
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "BraKet", new Type[0]);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }


    [Test]
    public void Equals_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (Proxied), "GenericToString");
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "GenericToString");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 2);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_Differing_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }

    [Test]
    public void Equals_GenericMethodFromBaseType_NonGenericOverloadInType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", new[] { typeof (int), typeof (int) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }


    private class Test1
    {
      public string OverloadedGenericToString<Tx> (Tx tx)
      {
        return "Test1:" + tx;
      }
      
      public string OverloadedGenericToString<Tx, Ty> (Tx tx, Ty ty)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx,Ty> (string s, Tx tx, object o, Ty ty)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }

      public string ComplexGenericArgumentTest<Tx, Ty> (Dictionary<Tx,Ty> d)
      {
        return "Test1.ComplexGenericArgumentTest:" + d;
      }
    }

    private class Test2
    {
      public string OverloadedGenericToString<Tx, Ty> (Ty ty, Tx tx)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx, Ty> (string s, Tx tx, object o, Ty ty)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }
    }

    private class Test3
    {
      public string OverloadedGenericToString<Tx, Ty> (Ty ty, Tx tx)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx, Ty> (string s, Ty ty, object o, Tx tx)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }
    }

    [Test]
    public void Equals_GenericMethodFromNonRelatedTypes ()
    {
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var methodWithSameSignature = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test1), "OverloadedGenericToString", 2);
      var methodWithSwappedGenericArguments = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test2), "OverloadedGenericToString", 2);

      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSameSignature), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSwappedGenericArguments), Is.False);
    }

    [Test]
    public void Equals_MixedArgumentGenericMethodFromNonRelatedTypes ()
    {
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test1), "MixedArgumentsTest", 2);
      var methodWithSameSignature = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test2), "MixedArgumentsTest", 2);
      var methodWithSwappedGenericArguments = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test3), "MixedArgumentsTest", 2);

      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSameSignature), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSwappedGenericArguments), Is.False);
    }


 

    [Test]
    public void Equals_GenericClass_MethodWithGenericClassArgumentsFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      var method = ScriptingHelper.GetAnyInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_GenericClass_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    // FAILURE: Comparing MethodInfo between generic method and generic method hiding method through "new" does not work !
    [Test]
    [Explicit]
    public void Equals_GenericClass_GenericMethodFromBaseType2 ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
      var methods = ScriptingHelper.GetAnyGenericInstanceMethodArray (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
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

      Assert.That (methodFromBaseType, Is.Not.EqualTo (methods[0]));
      Assert.That (methodFromBaseType, Is.Not.EqualTo (methods[1]));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[1]), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[0]), Is.True);
    }
 
  }

 
}
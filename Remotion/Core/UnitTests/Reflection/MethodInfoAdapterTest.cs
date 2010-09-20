// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class MethodInfoAdapterTest
  {
    private MethodInfo _method;
    private MethodInfo _explicitInterfaceImplementationMethod;
    private MethodInfo _implicitInterfaceImplementationMethod;

    private MethodInfoAdapter _adapter;
    private MethodInfoAdapter _explicitInterfaceAdapter;
    private MethodInfoAdapter _implicitInterfaceAdapter;


    [SetUp]
    public void SetUp ()
    {
      _method = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod");
      _adapter = new MethodInfoAdapter (_method);

      _explicitInterfaceImplementationMethod = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceMethod",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceAdapter = new MethodInfoAdapter (_explicitInterfaceImplementationMethod);

      _implicitInterfaceImplementationMethod = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "ImplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = new MethodInfoAdapter (_implicitInterfaceImplementationMethod);
    }

    [Test]
    public void MethodInfo ()
    {
      Assert.That (_adapter.MethodInfo, Is.SameAs (_method));
      Assert.That (_implicitInterfaceAdapter.MethodInfo, Is.SameAs (_implicitInterfaceImplementationMethod));
      Assert.That (_explicitInterfaceAdapter.MethodInfo, Is.SameAs (_explicitInterfaceImplementationMethod));
    }


    [Test]
    public void Name ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_method.Name));
    }

    [Test]
    public void Name_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo (_implicitInterfaceImplementationMethod.Name));
    }

    [Test]
    public void Name_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.Name, Is.EqualTo (_explicitInterfaceImplementationMethod.Name));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (_method.DeclaringType));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (
          _adapter.GetCustomAttribute<SampleAttribute> (true), Is.EqualTo (AttributeUtility.GetCustomAttribute<SampleAttribute> (_method, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (
          _adapter.GetCustomAttributes<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttributes<SampleAttribute> (_method, false)));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SampleAttribute> (_method, true)));
    }

    [Test]
    public void GetMethodInfo ()
    {
      Assert.That (_adapter.MethodInfo, Is.SameAs (_method));
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_adapter, Is.EqualTo (new MethodInfoAdapter (_method)));
      Assert.AreNotEqual (_adapter, new MethodInfoAdapter (typeof (ClassWithOverridingMember).GetMethod ("BaseMethod")));
    }

    [Test]
    public void GetReturnType ()
    {
      Assert.That (_adapter.ReturnType, Is.EqualTo (_method.ReturnType));
    }

    [Test]
    public void GetName ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_method.Name));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_method.DeclaringType));
    }

    [Test]
    public void Invoke_BaseMethod ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = new MethodInfoAdapter (methodInfo);
      var result = adapter.Invoke (new ClassWithBaseMember(), new object[] { });

      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (TargetException), ExpectedMessage = "Object does not match target type.")]
    public void Invoke_WrongInstanceForMethod_GetExceptionFromReflectionApi()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = new MethodInfoAdapter (methodInfo);
      var result = adapter.Invoke ("Test", new object[0]);

      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    public void FindInterfaceImplementation_ImplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      Assert.That (
          ((MethodInfoAdapter) implementation).MethodInfo,
          Is.EqualTo (typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_ExplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedPropertyGetter = typeof (ClassWithReferenceType<object>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((MethodInfoAdapter) implementation).MethodInfo, Is.EqualTo (expectedPropertyGetter));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method is not an interface method.")]
    public void FindInterfaceImplementation_NonInterfaceMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The implementationType parameter must not be an interface.\r\nParameter name: implementationType")]
    public void FindInterfaceImplementation_ImplementationIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      adapter.FindInterfaceImplementation (typeof (IInterfaceWithReferenceType<object>));
    }

    [Test]
    public void FindInterfaceImplementation_ImplementationIsNotAssignableToTheInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceImplementation (typeof (object));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "This method is itself an interface member, so it cannot have an interface declaration.")]
    public void FindInterfaceDeclaration_DeclaringTypeIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      new MethodInfoAdapter (methodInfo).FindInterfaceDeclaration();
    }

    [Test]
    public void FindInterfaceDeclaration_ImplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitImplementation_FromBaseType ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration ();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceDeclaration_ComparesMethodsWithoutReflectedTypes ()
    {
      var methodInfo = typeof (DerivedClassWithReferenceType<object>).GetMethod ("ImplicitInterfaceMethod");
      var adapter = new MethodInfoAdapter (methodInfo);

      Assert.That (methodInfo.ReflectedType, Is.Not.SameAs (methodInfo.DeclaringType));

      var result = adapter.FindInterfaceDeclaration ();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetMethod ("ImplicitInterfaceMethod")));
    }

    [Test]
    public void FindInterfaceDeclaration_NoImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("TestMethod");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindDeclaringProperty_PublicPropertyAccesor_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), "ImplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_PublicPropertyAccesor_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("set_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), "ImplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesor_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), "PrivateProperty", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesor_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("set_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), "PrivateProperty", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesorOfPublicProperty ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod("set_ReadOnlyNonPublicSetterScalar",BindingFlags.Instance|BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), "ReadOnlyNonPublicSetterScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_ExplicitlyImplementedInterfacePropertyAccessorInBaseType ()
    {
      var methodInfo =
          typeof (DerivedClassWithReferenceType<object>)
            .GetInterfaceMap (typeof (IInterfaceWithReferenceType<object>)).TargetMethods
            .Where (
                m => m.Name == "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar")
            .Single();
      var adapter = new MethodInfoAdapter (methodInfo);

      // We have a private property whose declaring type is different from the reflected type. It is not possible to get such a method via ordinary 
      // Reflection; only via GetInterfaceMap.
      Assert.That (methodInfo.DeclaringType, Is.SameAs (typeof (ClassWithReferenceType<object>)));
      Assert.That (methodInfo.ReflectedType, Is.SameAs (typeof (DerivedClassWithReferenceType<object>)));
      Assert.That (methodInfo.IsPrivate, Is.True);

      var result = adapter.FindDeclaringProperty ();

      CheckProperty (typeof (ClassWithReferenceType<object>), 
        "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_NoPropertyCanBeFound ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty ();

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetFastInvoker_PublicMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new ClassWithReferenceType<string>();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_PrivateMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new ClassWithReferenceType<string>();
      PrivateInvoke.SetNonPublicProperty (instance, "PrivateProperty", "Test");

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_DerivedClassPrivateMethod_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new DerivedClassWithReferenceType<string> ();
      PrivateInvoke.SetNonPublicProperty (instance, "PrivateProperty", "Test");

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>> ();

      Assert.That (result.GetType (), Is.EqualTo(typeof (Func<ClassWithReferenceType<string>, string>)));
      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_DerivedClassPrivateMethod_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("set_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new DerivedClassWithReferenceType<string> ();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Action<ClassWithReferenceType<string>, string>> ();

      result (instance, "Test");
      Assert.That(PrivateInvoke.GetNonPublicProperty (instance, "PrivateProperty"), Is.EqualTo("Test"));
      Assert.That (result.GetType (), Is.EqualTo(typeof (Action<ClassWithReferenceType<string>, string>)));
    }
    
    [Test]
    public void GetParameters_MethodWithoutParameters ()
    {
      Assert.That (_adapter.GetParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetParameters_MethodWithParameters ()
    {
      var method = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethodWithParameters");
       var adapter = new MethodInfoAdapter (method);

       Assert.That (adapter.GetParameters ().Length, Is.EqualTo (2));
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_adapter.Name, Is.EqualTo ("TestMethod"));
    }

    void CheckProperty (Type expectedDeclaringType, string expectedName, IPropertyInformation actualProperty)
    {
      Assert.That (actualProperty.Name, Is.EqualTo(expectedName));
      Assert.That (actualProperty.DeclaringType, Is.SameAs (expectedDeclaringType));
    }
  }
}
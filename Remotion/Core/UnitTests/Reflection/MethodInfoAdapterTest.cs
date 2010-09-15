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
    public void FindInterfaceImplementation_ImplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      Assert.That (
          ((MethodInfoAdapter) implementation).MethodInfo,
          Is.EqualTo (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod()));
    }

    [Test]
    public void FindInterfaceImplementation_ExplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar").GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedPropertyGetter = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      Assert.That (((MethodInfoAdapter) implementation).MethodInfo, Is.EqualTo (expectedPropertyGetter));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method is not an interface method.")]
    public void FindInterfaceImplementation_NonInterfaceMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The implementationType parameter must not be an interface.\r\nParameter name: implementationType")]
    public void FindInterfaceImplementation_ImplementationIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      adapter.FindInterfaceImplementation (typeof (IInterfaceWithReferenceType<object>));
    }

    [Test]
    public void FindInterfaceImplementation_ImplementationIsNotAssignableToTheInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceImplementation (typeof (object));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method is not an implementation method.")]
    public void FindInterfaceDeclaration_DeclaringTypeIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      new MethodInfoAdapter (methodInfo).FindInterfaceDeclaration();
    }

    [Test]
    public void FindInterfaceDeclaration_ImplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod()));
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindInterfaceDeclaration();

      Assert.That (
          ((MethodInfoAdapter) result).MethodInfo,
          Is.EqualTo (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar").GetGetMethod (true)));
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
    public void FindDeclaringType_PublicPropertyAccesor ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty (typeof (ClassWithReferenceType<object>));

      Assert.That (result.Name, Is.EqualTo ("ImplicitInterfaceScalar"));
    }

    [Test]
    public void FindDeclaringType_PrivatePropertyAccesor ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetProperty ("PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic)
          .GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty (typeof (ClassWithReferenceType<object>));

      Assert.That (result.Name, Is.EqualTo ("PrivateProperty"));
    }

    [Test]
    public void FindDeclaringType_PrivatePropertyAccesorOfPublicProperty ()
    {
      var methodInfo =
          typeof (ClassWithReferenceType<object>).GetProperty (
              "PrivateImplicitInterfaceScalarAccesor", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty (typeof (ClassWithReferenceType<object>));

      Assert.That (result.Name, Is.EqualTo ("PrivateImplicitInterfaceScalarAccesor"));
    }

    [Test]
    public void FindDeclaringType_ExplicitlyImplementedInterfacePropertyAccessorInBaseType ()
    {
      var methodInfo =
          typeof (DerivedClassWithReferenceType<object>).GetInterfaceMap (typeof (IInterfaceWithReferenceType<object>)).TargetMethods.Where (
              m => m.Name == "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar").
              Single();
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (
          result.Name,
          Is.EqualTo ("Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"));
    }

    [Test]
    public void FindDeclaringType_NoPropertyCanBeFound ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = new MethodInfoAdapter (methodInfo);

      var result = adapter.FindDeclaringProperty (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetFastInvoker_PublicMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod();
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new ClassWithReferenceType<string>();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result.GetType().IsSubclassOf (typeof (Delegate)));
      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_PrivateMethod ()
    {
      var methodInfo =
          typeof (ClassWithReferenceType<string>).GetProperty (
              "PrivateImplicitInterfaceScalarAccesor", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new ClassWithReferenceType<string>();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result.GetType().IsSubclassOf (typeof (Delegate)));
      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_DerivedClassPrivateMethod ()
    {
      var methodInfo =
          typeof (ClassWithReferenceType<string>).GetProperty (
              "PrivateImplicitInterfaceScalarAccesor", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);
      var instance = new DerivedClassWithReferenceType<string> ();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>> ();

      Assert.That (result.GetType ().IsSubclassOf (typeof (Delegate)));
      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object is not a delegate type.")]
    public void GetFastInvoker_NoDelegateType ()
    {
      var methodInfo =
          typeof (ClassWithReferenceType<string>).GetProperty (
              "PrivateImplicitInterfaceScalarAccesor", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod (true);
      var adapter = new MethodInfoAdapter (methodInfo);
      adapter.GetFastInvoker<object> ();
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
  }
}
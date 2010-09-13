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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class PropertyInfoAdapterTest
  {
    private PropertyInfo _property;
    private PropertyInfo _explicitInterfaceImplementationProperty;
    private PropertyInfo _implicitInterfaceImplementationProperty;

    private PropertyInfoAdapter _adapter;

    private PropertyInfoAdapter _explicitInterfaceAdapter;
    private PropertyInfoAdapter _implicitInterfaceAdapter;

    [SetUp]
    public void SetUp ()
    {
      _property = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      _adapter = new PropertyInfoAdapter (_property);

      _explicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceAdapter = new PropertyInfoAdapter (_explicitInterfaceImplementationProperty);

      _implicitInterfaceImplementationProperty = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "ImplicitInterfaceScalar",
          BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = new PropertyInfoAdapter (_implicitInterfaceImplementationProperty);
    }

    [Test]
    public void PropertyInfo ()
    {
      Assert.That (_adapter.PropertyInfo, Is.SameAs (_property));
      Assert.That (_implicitInterfaceAdapter.PropertyInfo, Is.SameAs (_implicitInterfaceImplementationProperty));
      Assert.That (_explicitInterfaceAdapter.PropertyInfo, Is.SameAs (_explicitInterfaceImplementationProperty));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That (_adapter.PropertyType, Is.EqualTo (_property.PropertyType));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_property.Name));
    }

    [Test]
    public void Name_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo (_implicitInterfaceImplementationProperty.Name));
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo ("ImplicitInterfaceScalar"));
    }

    [Test]
    public void Name_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.Name, Is.EqualTo (_explicitInterfaceImplementationProperty.Name));
      Assert.That (
          _explicitInterfaceAdapter.Name,
          Is.EqualTo ("Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (_property.DeclaringType));
    }

    [Test]
    public void CanBeSetFromOutside_Scalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Scalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyScalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyScalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyNonPublicSetterScalar ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyNonPublicSetterScalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitInterfaceScalar_FromImplementation ()
    {
      PropertyInfoAdapter adapter = _implicitInterfaceAdapter;

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (
          _adapter.GetCustomAttribute<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttribute<SampleAttribute> (_property, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (
          _adapter.GetCustomAttributes<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttributes<SampleAttribute> (_property, true)));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SampleAttribute> (_property, true)));
    }

    [Test]
    public void GetValue_WithIndexerProperty_OneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10 });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_OneParameter_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();
      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[0]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_TwoParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });

      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1) });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayNull ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[1]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_ThreeParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "TestException")]
    public void GetValue_WithIndexerProperty_ThreeParameters_UnwrapsTargetInvocationException ()
    {
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"]).Throw (new ApplicationException ("TestException"));

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
    }

    [Test]
    public void SetValue_ExplicitInterface_Integration ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType>();
      SimpleReferenceType value = new SimpleReferenceType();
      _explicitInterfaceAdapter.SetValue (instance, value, null);
      Assert.That (instance.ExplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithOneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10 });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[0]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithTwoParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1) });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[1]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithThreeParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "TestException")]
    public void SetValue_WithIndexerProperty_WithThreeParameters_UnwrapsTargetInvocationException ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"] = scalar).Throw (new ApplicationException ("TestException"));

      var interfaceDeclarationProperty = typeof (IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
    }

    [Test]
    public void Equals_ChecksPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter,
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty)));
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_explicitInterfaceImplementationProperty), _implicitInterfaceAdapter);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter.GetHashCode(),
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty).GetHashCode()));
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_explicitInterfaceImplementationProperty).GetHashCode(),
          _implicitInterfaceAdapter.GetHashCode());
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_adapter.DeclaringType));

      PropertyInfo propertyInfo = typeof (ClassWithOverridingMember).GetProperty ("BaseProperty");
      PropertyInfoAdapter overrideAdapter = new PropertyInfoAdapter (propertyInfo);
      Assert.AreNotEqual (overrideAdapter.DeclaringType, overrideAdapter.GetOriginalDeclaringType());
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (overrideAdapter.DeclaringType.BaseType));
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (typeof (ClassWithBaseMember)));
    }

    [Test]
    public void GetGetMethod_PublicProperty ()
    {
      var getMethod = _adapter.GetGetMethod (false);
      var expectedMethod = new MethodInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("get_NotVisibleAttributeScalar"));

      Assert.That (getMethod, Is.Not.Null);
      Assert.That (getMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetGetMethod_PrivateProperty_NonPublicFalse ()
    {
      var getMethod = _explicitInterfaceAdapter.GetGetMethod (false);

      Assert.That (getMethod, Is.Null);
    }

    [Test]
    public void GetGetMethod_PrivateProperty_NonPublicTrue ()
    {
      var getMethod = _explicitInterfaceAdapter.GetGetMethod (true);

      var expectedMethod = new MethodInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar", 
          BindingFlags.Instance | BindingFlags.NonPublic));

      Assert.That (getMethod, Is.Not.Null);
      Assert.That (getMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetGetMethod_NonExistingMethod ()
    {
      var adapter = new PropertyInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceWriteOnlyScalar"));

      var getMethod = adapter.GetGetMethod (false);

      Assert.That (getMethod, Is.Null);
    }

    [Test]
    public void GetSetMethod_PublicProperty ()
    {
      var setMethod = _adapter.GetSetMethod (false);
      
      var expectedMethod = new MethodInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("set_NotVisibleAttributeScalar"));
      Assert.That (setMethod, Is.Not.Null);
      Assert.That (setMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetSetMethod_PrivateProperty_NonPublicFalse ()
    {
      var setMethod = _explicitInterfaceAdapter.GetSetMethod (false);

      Assert.That (setMethod, Is.Null);
    }

    [Test]
    public void GetSetMethod_PrivateProperty_NonPublicTrue ()
    {
      var setMethod = _explicitInterfaceAdapter.GetSetMethod (true);

      var expectedMethod = new MethodInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.set_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic));

      Assert.That (setMethod, Is.Not.Null);
      Assert.That (setMethod, Is.EqualTo (expectedMethod));
    }

    [Test]
    public void GetSetMethod_NonExistingMethod ()
    {
      var adapter = new PropertyInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceReadOnlyScalar"));

      var setMethod = adapter.GetSetMethod (false);

      Assert.That (setMethod, Is.Null);
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ImplicitImplementation ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar", 
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_OnBaseClass ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_GetterOnly ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceReadOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ExplicitImplementation_SetterOnly ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ExplicitInterfaceWriteOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedProperty = typeof (ClassWithReferenceType<object>).GetProperty (
          "Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceWriteOnlyScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (expectedProperty));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_ImplementedOnBaseClass ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceReadOnlyScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceReadOnlyScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_OverriddenOnDerivedClass ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));
      var implementation = adapter.FindInterfaceImplementation (typeof (DerivedClassWithReferenceType<object>));

      Assert.That (((PropertyInfoAdapter) implementation).PropertyInfo, Is.EqualTo (typeof (DerivedClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar")));
    }

    [Test]
    public void FindInterfaceImplementation_InterfaceProperty_NotImplemented ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));
      
      var implementation = adapter.FindInterfaceImplementation (typeof (object));

      Assert.That (implementation, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The implementationType parameter must not be an interface.\r\nParameter name: implementationType")]
    public void FindInterfaceImplementation_InterfaceProperty_ImplementationIsInterface ()
    {
      var adapter = new PropertyInfoAdapter (typeof (IInterfaceWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      adapter.FindInterfaceImplementation (typeof (IInterfaceWithReferenceType<object>));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method is not an interface method.")]
    public void FindInterfaceImplementation_NonInterfaceProperty ()
    {
      var adapter = new PropertyInfoAdapter (typeof (ClassWithReferenceType<object>).GetProperty ("ImplicitInterfaceScalar"));

      adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));
    }
    
    private void AssertCanSet (PropertyInfoAdapter adapter, object instance, SimpleReferenceType value)
    {
      adapter.SetValue (instance, value, null);
      Assert.That (adapter.GetValue (instance, null), Is.SameAs (value));
    }

    private void AssertCanNotSet (PropertyInfoAdapter adapter, object instance, SimpleReferenceType value)
    {
      try
      {
        adapter.SetValue (instance, value, null);
      }
      catch (ArgumentException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Property set method not found."));
      }
    }
  }
}
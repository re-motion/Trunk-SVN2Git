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
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.Properties
{
  [TestFixture]
  public class BindableObjectPropertyInfoAdapterTest
  {
    private PropertyInfo _property;
    private PropertyInfo _explicitInterfaceImplementationProperty;
    private PropertyInfo _explicitInterfaceDeclarationProperty;
    private PropertyInfo _implicitInterfaceImplementationProperty;
    private PropertyInfo _implicitInterfaceDeclarationProperty;

    private PropertyInfoAdapter _adapter;

    private PropertyInfoAdapter _explicitInterfaceAdapter;
    private PropertyInfoAdapter _implicitInterfaceAdapter;

    [SetUp]
    public void SetUp ()
    {
      _property = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      _adapter = new PropertyInfoAdapter (_property, null);

      _explicitInterfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ExplicitInterfaceScalar");
      _explicitInterfaceAdapter = new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty);

      _implicitInterfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      _implicitInterfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "ImplicitInterfaceScalar",
          BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty);
    }

    [Test]
    public void PropertyInfo ()
    {
      Assert.That (_adapter.PropertyInfo, Is.SameAs (_property));
      Assert.That (_implicitInterfaceAdapter.PropertyInfo, Is.SameAs (_implicitInterfaceImplementationProperty));
      Assert.That (_explicitInterfaceAdapter.PropertyInfo, Is.SameAs (_explicitInterfaceImplementationProperty));
    }

    [Test]
    public void InterfacePropertyInfo ()
    {
      Assert.That (_adapter.InterfacePropertyInfo, Is.Null);
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_implicitInterfaceDeclarationProperty));
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_explicitInterfaceDeclarationProperty));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Parameter must be a property declared on an interface.\r\nParameter name: interfacePropertyInfo")]
    public void InvalidInterfaceProperty ()
    {
      new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceImplementationProperty);
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
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (_property.DeclaringType));
    }

    [Test]
    public void CanBeSetFromOutside_Scalar ()
    {
      PropertyInfo propertyInfo = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Scalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyScalar ()
    {
      PropertyInfo propertyInfo = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyScalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ReadOnlyNonPublicSetterScalar ()
    {
      PropertyInfo propertyInfo = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyNonPublicSetterScalar");
      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitInterfaceScalar ()
    {
      PropertyInfoAdapter adapter = _explicitInterfaceAdapter;

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitInterfaceReadOnlyScalar ()
    {
      PropertyInfo propertyInfo = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar",
          BindingFlags.NonPublic | BindingFlags.Instance);

      var adapter = new PropertyInfoAdapter (propertyInfo);

      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitInterfaceScalar_FromImplementation ()
    {
      PropertyInfoAdapter adapter = _implicitInterfaceAdapter;

      Assert.That (adapter.CanBeSetFromOutside, Is.True);
      AssertCanSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitInterfaceReadOnlyScalar_FromReadWriteImplementation ()
    {
      var declarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceReadOnlyScalar");
      var implementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty (
          "ImplicitInterfaceScalar",
          BindingFlags.Public | BindingFlags.Instance);
      var adapter = new PropertyInfoAdapter (implementationProperty, declarationProperty);

      // TODO 1439: Change this to Is.True/AssertCanSet
      Assert.That (adapter.CanBeSetFromOutside, Is.False);
      AssertCanNotSet (adapter, new TestDomain.ClassWithReferenceType<SimpleReferenceType>(), new SimpleReferenceType());
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
    public void GetValue_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock.ImplicitInterfaceScalar).Return (scalar);
      instanceMock.Replay();

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, null);
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValue_ExplicitInterface_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock.ExplicitInterfaceScalar).Return (scalar);
      instanceMock.Replay();

      object actualScalar = _explicitInterfaceAdapter.GetValue (instanceMock, null);
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValue_ExplicitInterface_Integration ()
    {
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instance = new TestDomain.ClassWithReferenceType<SimpleReferenceType>();
      instance.ExplicitInterfaceScalar = new SimpleReferenceType();
      Assert.That (_explicitInterfaceAdapter.GetValue (instance, null), Is.EqualTo (instance.ExplicitInterfaceScalar));
    }

    [Test]
    public void GetValue_WithIndexerProperty_OneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10 });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_OneParameter_IndexParameterArrayNull ()
    {
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_OneParameter_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[0]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_TwoParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1) });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayNull ()
    {
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void GetValue_WithIndexerProperty_TwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceStub, new object[1]);
    }

    [Test]
    public void GetValue_WithIndexerProperty_ThreeParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"]).Return (scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      object actualScalar = _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      Assert.That (actualScalar, Is.SameAs (scalar));
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "TestException")]
    public void GetValue_WithIndexerProperty_ThreeParameters_UnwrapsTargetInvocationException ()
    {
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"]).Throw (new ApplicationException ("TestException"));

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.GetValue (instanceMock, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
    }

    [Test]
    public void SetValue_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock.ImplicitInterfaceScalar = scalar);
      instanceMock.Replay();

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, null);
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValue_ExplicitInterface_UsesValueProperty ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock.ExplicitInterfaceScalar = scalar);
      instanceMock.Replay();

      _explicitInterfaceAdapter.SetValue (instanceMock, scalar, null);
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValue_ExplicitInterface_Integration ()
    {
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instance = new TestDomain.ClassWithReferenceType<SimpleReferenceType>();
      SimpleReferenceType value = new SimpleReferenceType();
      _explicitInterfaceAdapter.SetValue (instance, value, null);
      Assert.That (instance.ExplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithOneParameter ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10 });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithOneParameter_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[0]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithTwoParameters ()
    {
      var scalar = new SimpleReferenceType();
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1)] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1) });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayNull ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, null);
    }

    [Test]
    [ExpectedException (typeof (TargetParameterCountException), ExpectedMessage = "Parameter count mismatch.")]
    public void SetValue_WithIndexerProperty_WithTwoParameters_IndexParameterArrayLengthMismatch ()
    {
      var scalar = new SimpleReferenceType();
      var instanceStub = MockRepository.GenerateStub<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceStub, scalar, new object[1]);
    }

    [Test]
    public void SetValue_WithIndexerProperty_WithThreeParameters ()
    {
      SimpleReferenceType scalar = new SimpleReferenceType();
      TestDomain.IInterfaceWithReferenceType<SimpleReferenceType> instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>>();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"] = scalar);
      instanceMock.Replay();

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
      instanceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ApplicationException), ExpectedMessage = "TestException")]
    public void SetValue_WithIndexerProperty_WithThreeParameters_UnwrapsTargetInvocationException ()
    {
      var scalar = new SimpleReferenceType ();
      var instanceMock = MockRepository.GenerateMock<TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>> ();
      instanceMock.Expect (mock => mock[10, new DateTime (2000, 1, 1), "foo"] = scalar).Throw (new ApplicationException ("TestException"));

      var interfaceDeclarationProperty = typeof (TestDomain.IInterfaceWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      var interfaceImplementationProperty = typeof (TestDomain.ClassWithReferenceType<SimpleReferenceType>)
          .GetProperty ("Item", new[] { typeof (int), typeof (DateTime), typeof (string) });
      _implicitInterfaceAdapter = new PropertyInfoAdapter (interfaceImplementationProperty, interfaceDeclarationProperty);

      _implicitInterfaceAdapter.SetValue (instanceMock, scalar, new object[] { 10, new DateTime (2000, 1, 1), "foo" });
    }

    [Test]
    public void Equals_ChecksPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter,
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty)));
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty), _implicitInterfaceAdapter);
    }

    [Test]
    public void Equals_ChecksValuePropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter,
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty)));
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty), _implicitInterfaceAdapter);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.That (
          _implicitInterfaceAdapter.GetHashCode(),
          Is.EqualTo (new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty).GetHashCode()));
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_explicitInterfaceImplementationProperty, _implicitInterfaceDeclarationProperty).GetHashCode(),
          _implicitInterfaceAdapter.GetHashCode());
      Assert.AreNotEqual (
          new PropertyInfoAdapter (_implicitInterfaceImplementationProperty, _explicitInterfaceDeclarationProperty).GetHashCode(),
          _implicitInterfaceAdapter.GetHashCode());
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_adapter.DeclaringType));

      PropertyInfo propertyInfo = typeof (ClassWithOverridingProperty).GetProperty ("BaseProperty");
      PropertyInfoAdapter overrideAdapter = new PropertyInfoAdapter (propertyInfo);
      Assert.AreNotEqual (overrideAdapter.DeclaringType, overrideAdapter.GetOriginalDeclaringType());
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (overrideAdapter.DeclaringType.BaseType));
      Assert.That (overrideAdapter.GetOriginalDeclaringType(), Is.EqualTo (typeof (ClassWithBaseProperty)));
    }

    [Test]
    public void InterfaceProperty_Null ()
    {
      Assert.That (_adapter.InterfacePropertyInfo, Is.Null);
    }

    [Test]
    public void DeclaringInterfaceType_NonNull_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.Not.Null);
      Assert.That (_explicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_explicitInterfaceDeclarationProperty));
    }

    [Test]
    public void DeclaringInterfaceType_NonNull_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.Not.Null);
      Assert.That (_implicitInterfaceAdapter.InterfacePropertyInfo, Is.SameAs (_implicitInterfaceDeclarationProperty));
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
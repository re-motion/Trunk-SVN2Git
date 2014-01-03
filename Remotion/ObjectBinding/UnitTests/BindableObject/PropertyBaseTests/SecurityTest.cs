// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Security;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyBaseTests
{
  [TestFixture]
  public class SecurityTest : TestBase
  {
    private MockRepository _mocks;
    private BindableObjectProvider _businessObjectProvider;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private IBusinessObjectProperty _securableProperty;
    private IBusinessObjectProperty _securableExplicitInterfaceProperty;
    private IBusinessObjectProperty _nonSecurableProperty;
    private IBusinessObjectProperty _nonSecurablePropertyReadOnly;
    private IBusinessObjectProperty _propertyWithoutSecurity;
    private IBusinessObject _securableObject;
    private IBusinessObject _nonSecurableObject;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _mocks = new MockRepository ();
      _businessObjectProvider = new BindableObjectProvider();
      _mockObjectSecurityAdapter = _mocks.StrictMock<IObjectSecurityAdapter>();

      _securableObject = (IBusinessObject) ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>>(ParamList.Create (_mocks.StrictMock<IObjectSecurityStrategy>()));

      _nonSecurableObject = (IBusinessObject) ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);

      IDefaultValueStrategy instance = new BindableObjectDefaultValueStrategy ();
      _securableProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (SecurableClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType), typeof (SimpleReferenceType), null, false, false, instance, _mockObjectSecurityAdapter));

      _propertyWithoutSecurity = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (SecurableClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType), typeof (SimpleReferenceType), null, false, false, instance, (IObjectSecurityAdapter) null));

      _securableExplicitInterfaceProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
                  "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"),
              typeof (SimpleReferenceType), typeof (SimpleReferenceType), null, false, false, instance, _mockObjectSecurityAdapter));
      
      _nonSecurablePropertyReadOnly = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyScalar"),
              typeof (SimpleReferenceType), typeof (SimpleReferenceType), null, false, true, instance, _mockObjectSecurityAdapter));
      
      _nonSecurableProperty = new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType), typeof (SimpleReferenceType), null, false, false, instance, _mockObjectSecurityAdapter));
    }

    [Test]
    public void IsAccessibleWithoutObjectSecurityProvider ()
    {
      _mocks.ReplayAll();

      bool isAccessible = _propertyWithoutSecurity.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.That (isAccessible, Is.True);
    }

    [Test]
    public void IsAccessible ()
    {
      ExpectHasAccessOnGetAccessor (true);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.That (isAccessible, Is.True);
    }

    [Test]
    public void IsNotAccessible ()
    {
      ExpectHasAccessOnGetAccessor (false);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.That (isAccessible, Is.False);
    }

    [Test]
    public void IsAccessibleOnExplicitInterfaceProperty ()
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnGetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableExplicitInterfaceProperty).PropertyInfo)).Return (true);

      _mocks.ReplayAll ();

      bool isAccessible = _securableExplicitInterfaceProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll ();
      Assert.That (isAccessible, Is.True);
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isAccessible = _nonSecurableProperty.IsAccessible (_nonSecurableObject.BusinessObjectClass, _nonSecurableObject);

      _mocks.VerifyAll();
      Assert.That (isAccessible, Is.True);
    }

    [Test]
    public void IsNotReadOnlyWithoutObjectSecurityProvider ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _propertyWithoutSecurity.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.That (isReadOnly, Is.False);
    }

    [Test]
    public void IsReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (false);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.That (isReadOnly, Is.True);
    }

    [Test]
    public void IsReadOnlyOnExplicitInterfaceProperty ()
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnSetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableExplicitInterfaceProperty).PropertyInfo)).Return (false);

      _mocks.ReplayAll ();

      bool isReadOnly = _securableExplicitInterfaceProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll ();
      Assert.That (isReadOnly, Is.True);
    }

    [Test]
    public void IsNotReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (true);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.That (isReadOnly, Is.False);
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurableProperty.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.That (isReadOnly, Is.False);
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurablePropertyReadOnly.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.That (isReadOnly, Is.True);
    }

    private void ExpectHasAccessOnGetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnGetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo)).Return (returnValue);
    }

    private void ExpectHasAccessOnSetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnSetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo)).Return (returnValue);
    }
  }
}

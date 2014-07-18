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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetDisplayName : TestBase
  {
    private IBindablePropertyReadAccessStrategy _bindablePropertyReadAccessStrategyMock;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _bindablePropertyReadAccessStrategyMock = MockRepository.GenerateStrictMock<IBindablePropertyReadAccessStrategy>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _bindablePropertyReadAccessStrategyMock);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty));

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithOverriddenDisplayName> (ParamList.Empty);

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndAccessGranted ()
    {
      var securableObject = (IBusinessObject) ObjectFactory.Create<ClassWithOverriddenDisplayName>();
      var bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      var displayNameProperty = ((PropertyBase)bindableObjectMixin.BusinessObjectClass.GetPropertyDefinition ("DisplayName"));
      _bindablePropertyReadAccessStrategyMock
          .Expect (_ => _.CanRead (securableObject, displayNameProperty))
          .Return (true);

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _bindablePropertyReadAccessStrategyMock.VerifyAllExpectations();
      Assert.That (actual, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndWithAccessDenied ()
    {
      var securableObject = (IBusinessObject) ObjectFactory.Create<ClassWithOverriddenDisplayName>();
      var bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      var displayNameProperty = ((PropertyBase)bindableObjectMixin.BusinessObjectClass.GetPropertyDefinition ("DisplayName"));
      _bindablePropertyReadAccessStrategyMock
          .Expect (_ => _.CanRead (securableObject, displayNameProperty))
          .Return (false);

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _bindablePropertyReadAccessStrategyMock.VerifyAllExpectations();
      Assert.That (actual, Is.EqualTo ("×"));
    }

    [Test]
    public void DisplayNameSafe_WithoutOverriddenDisplayName ()
    {
      var securableObject = (IBusinessObject) ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>();
      var bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _bindablePropertyReadAccessStrategyMock.VerifyAllExpectations ();
      Assert.That (
          actual,
         Is.StringStarting("Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithReferenceType"));
    }
  }
}

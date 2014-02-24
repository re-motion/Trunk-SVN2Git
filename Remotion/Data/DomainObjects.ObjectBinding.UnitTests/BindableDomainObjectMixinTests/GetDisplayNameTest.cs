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
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Security;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class GetDisplayNameTest : ObjectBindingTestBase
  {
    private MockRepository _mockRepository;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _mockObjectSecurityAdapter = _mockRepository.StrictMock<IObjectSecurityAdapter>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple <IObjectSecurityAdapter> (() => _mockObjectSecurityAdapter);
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
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (SampleBindableMixinDomainObject.NewObject ());

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain.SampleBindableMixinDomainObject, Remotion.Data.DomainObjects.ObjectBinding.UnitTests"));
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) SampleBindableMixinDomainObjectWithOverriddenDisplayName.NewObject();

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndAccessGranted ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = SecurableBindableMixinDomainObjectWithOverriddenDisplayName.NewObject (stubSecurityStrategy);
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (securableObject);
      var displayNamePropertyInformation = ((PropertyBase) bindableObjectMixin.BusinessObjectClass.GetPropertyDefinition ("DisplayName")).PropertyInfo;
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, displayNamePropertyInformation)).Return (true);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndWithAccessDenied ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = SecurableBindableMixinDomainObjectWithOverriddenDisplayName.NewObject (stubSecurityStrategy);
      BindableDomainObjectMixin bindableObjectMixin = Mixin.Get<BindableDomainObjectMixin> (securableObject);
      var displayNamePropertyInformation = ((PropertyBase) bindableObjectMixin.BusinessObjectClass.GetPropertyDefinition ("DisplayName")).PropertyInfo;
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, displayNamePropertyInformation)).Return (false);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("×"));
    }
  }
}

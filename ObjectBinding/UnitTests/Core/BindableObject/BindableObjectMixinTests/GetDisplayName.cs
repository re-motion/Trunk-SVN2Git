/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetDisplayName : TestBase
  {
    private MockRepository _mockRepository;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _mockObjectSecurityAdapter = _mockRepository.CreateMock<IObjectSecurityAdapter>();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), _mockObjectSecurityAdapter);
    }

    public override void TearDown ()
    {
      base.TearDown();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithOverriddenDisplayName>().With();

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndAccessGranted ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (true);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheDisplayName"));
    }

    [Test]
    public void DisplayNameSafe_WithOverriddenDisplayNameAndWithAccessDenied ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (securableObject, "DisplayName")).Return (false);
      _mockRepository.ReplayAll();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("×"));
    }

    [Test]
    public void DisplayNameSafe_WithoutOverriddenDisplayName ()
    {
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy> ();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>> ().With (stubSecurityStrategy);
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (securableObject);
      _mockRepository.ReplayAll ();

      string actual = ((IBusinessObject) bindableObjectMixin).DisplayNameSafe;

      _mockRepository.VerifyAll ();
      Assert.That (
          actual,
          NUnit.Framework.SyntaxHelpers.Text.StartsWith ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.SecurableClassWithReferenceType"));
    }
  }
}

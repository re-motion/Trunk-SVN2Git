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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
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
      _mockObjectSecurityAdapter = _mockRepository.StrictMock<IObjectSecurityAdapter>();
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
      BindableObjectMixin bindableObjectMixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty));

      Assert.That (
          ((IBusinessObject) bindableObjectMixin).DisplayName,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
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
      IObjectSecurityStrategy stubSecurityStrategy = _mockRepository.Stub<IObjectSecurityStrategy>();
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>(ParamList.Create (stubSecurityStrategy));
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
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithOverriddenDisplayName>(ParamList.Create (stubSecurityStrategy));
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
      ISecurableObject securableObject = ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>> (ParamList.Create (stubSecurityStrategy));
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

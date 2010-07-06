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
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.MemberResolverTests
{
  [TestFixture]
  public class GetInstanceMethodInformationTest
  {
    private ReflectionBasedMemberResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ReflectionBasedMemberResolver();
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance);

      Assert.That (methodInformation, Is.InstanceOfType(typeof(NullMethodInformation)));
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance);

      Assert.AreSame (methodInformation, _resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance);
      var expectedMethodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Load",new Type[] {}));

      Assert.IsNotNull (methodInformation);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance);

      Assert.That (
          methodInformation, Is.SameAs(_resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance)));
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Delete", MemberAffiliation.Instance);
      var expectedMethodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Delete", new Type[] {typeof(int)}));

      Assert.IsNotNull (methodInformation);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Show", MemberAffiliation.Instance);
      var expectedMethodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Show"));

      Assert.IsNotNull (methodInformation);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_OverriddenMethodFromBaseMethod ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Record", MemberAffiliation.Instance);
      var expectedMethodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Record"));

      Assert.IsNotNull (methodInformation);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void Test_NotExistingMethod ()
    {
      _resolver.GetMethodInformation (typeof (SecurableObject), "Sve", MemberAffiliation.Instance);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
      ExpectedMessage = "The BaseDemandPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Send' exists in class 'Remotion.Security.UnitTests.Core.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: memberName")]
    public void Test_MethodDeclaredOnBaseAndDerivedClass ()
    {
      _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Send", MemberAffiliation.Instance);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
      ExpectedMessage = "The BaseDemandPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Print' exists in class 'Remotion.Security.UnitTests.Core.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: memberName")]
    public void Test_OverriddenMethods ()
    {
      _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Print", MemberAffiliation.Instance);
    }

    [Test]
    public void Test_MethodWithMultipleAccessTypes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Close", MemberAffiliation.Instance);

      Assert.That (methodInformation, Is.Not.Null);
    }
  }
}
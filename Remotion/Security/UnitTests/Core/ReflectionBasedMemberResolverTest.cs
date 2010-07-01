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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ReflectionBasedMemberResolverTest
  {
    [Test]
    public void GetMethodInfoAdapter ()
    {
      var expectedMethodInfoAdapter = new MethodInfoAdapter (typeof (SecurableObjectWithSecuredInstanceMethods).GetMethod ("InstanceMethod", new Type[] {}));

      var resolver = new ReflectionBasedMemberResolver();
      var result = resolver.GetMethodInformation (typeof (SecurableObjectWithSecuredInstanceMethods), "InstanceMethod", MemberAffiliation.Instance);

      Assert.That (result, Is.TypeOf (typeof (MethodInfoAdapter)));
      Assert.That (result, Is.EqualTo (expectedMethodInfoAdapter));
    }

    [Test]
    public void GetPropertyInfoAdapter ()
    {
      var expectedPropertyInfoAdapter = new PropertyInfoAdapter (typeof (SecurableObjectWithSecuredProperties).GetProperty ("SecretProperty"));

      var resolver = new ReflectionBasedMemberResolver ();
      var result = resolver.GetPropertyInformation (typeof (SecurableObjectWithSecuredProperties), "SecretProperty");

      Assert.That (result, Is.TypeOf (typeof (PropertyInfoAdapter)));
      Assert.That (result, Is.EqualTo (expectedPropertyInfoAdapter));
    }

    [Test]
    public void GetMethodInfoAdapter_StaticMethod ()
    {
      var expectedMethodInfoAdapter = new MethodInfoAdapter (typeof (SecurableObjectWithSecuredStaticMethods).GetMethod ("StaticMethod",new Type[] {}));

      var resolver = new ReflectionBasedMemberResolver();
      var result = resolver.GetMethodInformation (typeof (SecurableObjectWithSecuredStaticMethods), "StaticMethod", MemberAffiliation.Static);

      Assert.That (result, Is.TypeOf (typeof (MethodInfoAdapter)));
      Assert.That (result, Is.EqualTo (expectedMethodInfoAdapter));
    }
  }
}
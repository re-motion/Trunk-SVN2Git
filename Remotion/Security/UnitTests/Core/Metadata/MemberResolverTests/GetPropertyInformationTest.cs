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
  public class GetPropertyInformationTest
  {
    private ReflectionBasedMemberResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ReflectionBasedMemberResolver();
    }

    [Test]
    public void Test_PropertyWithoutAttributes ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (typeof (SecurableObject), "IsEnabled");

      Assert.That (propertyInformation, Is.InstanceOfType (typeof (NullPropertyInformation)));
    }

    [Test]
    public void Test_CachePropertyWithoutAttributes ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (typeof (SecurableObject), "IsEnabled");

      Assert.AreSame (propertyInformation, _resolver.GetPropertyInformation (typeof (SecurableObject), "IsEnabled"));
    }


    [Test]
    public void Test_PropertyWithOneAttribute ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (typeof (SecurableObject), "IsVisible");
      var expectedPropertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsVisible"));

      Assert.That (propertyInformation, Is.Not.Null);
      Assert.That (propertyInformation, Is.EqualTo (expectedPropertyInformation));
    }

    [Test]
    public void Test_CacheForPropertyWithOneAttribute ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (typeof (SecurableObject), "IsVisible");

      Assert.AreSame (propertyInformation, _resolver.GetPropertyInformation (typeof (SecurableObject), "IsVisible"));
    }

    [Test]
    public void Test_NonPublicPropertyWithOneAttribute ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (typeof (SecurableObject), "NonPublicProperty");
      var expectedPropertyInformation =
          new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("NonPublicProperty", BindingFlags.NonPublic | BindingFlags.Instance));

      Assert.That (propertyInformation, Is.Not.Null);
      Assert.That (propertyInformation, Is.EqualTo (expectedPropertyInformation));
    }


    [Test]
    public void Test_ExplicitInterfacePropertyWithOneAttribute ()
    {
      var propertyInformation = _resolver.GetPropertyInformation (
          typeof (SecurableObject),
          typeof (IInterfaceWithProperty).FullName + ".InterfaceProperty");
      var expectedPropertyInformation =
          new PropertyInfoAdapter (
              typeof (SecurableObject).GetProperty (
                  typeof (IInterfaceWithProperty).FullName + ".InterfaceProperty", BindingFlags.NonPublic | BindingFlags.Instance));

      Assert.That (propertyInformation, Is.Not.Null);
      Assert.That (propertyInformation, Is.EqualTo (expectedPropertyInformation));
    }
  }
}
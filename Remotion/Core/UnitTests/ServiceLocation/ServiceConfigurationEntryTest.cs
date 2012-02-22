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
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceConfigurationEntryTest
  {
    [Test]
    public void Initialize ()
    {
      var serviceImplementation = new ServiceConfigurationEntry.ServiceImplementationInfo (
          typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), serviceImplementation);

      var serviceImplementationInfo = serviceConfigurationEntry.ImplementationInfo;
      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceImplementationInfo.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceImplementationInfo.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void CreateFromAttribute ()
    {
      var attribute = new ConcreteImplementationAttribute (
          "Remotion.UnitTests.ServiceLocation.TestDomain.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>")
                      { Lifetime = LifetimeKind.Singleton };

      var serviceConfigurationEntry = ServiceConfigurationEntry.CreateFromAttribute (
          typeof (ITestSingletonConcreteImplementationAttributeType),
          attribute);

      var serviceImplementationInfo = serviceConfigurationEntry.ImplementationInfo;
      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceImplementationInfo.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceImplementationInfo.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void ImplementationInfo_Equals ()
    {
      var implementation0 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (object), LifetimeKind.Instance);
      var implementation1 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (object), LifetimeKind.Instance);
      var implementation2 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (string), LifetimeKind.Instance);
      var implementation3 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (object), LifetimeKind.Singleton);

      Assert.That (implementation0, Is.EqualTo (implementation1));
      Assert.That (implementation0, Is.Not.EqualTo (implementation2));
      Assert.That (implementation0, Is.Not.EqualTo (implementation3));
    }

    [Test]
    public void ImplementationInfo_GetHashCode ()
    {
      var implementation0 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (object), LifetimeKind.Instance);
      var implementation1 = new ServiceConfigurationEntry.ServiceImplementationInfo (typeof (object), LifetimeKind.Instance);

      Assert.That (implementation0.GetHashCode (), Is.EqualTo (implementation1.GetHashCode ()));
    }
  }
}
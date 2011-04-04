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
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using Remotion.Implementation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using System.Linq;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceConfigurationDiscoveryServiceTest
  {
    [Test]
    public void GetDefaultConfiguration ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Expect (stub => stub.GetTypes (null, false)).Return (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub);

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.First ();
      Assert.That (serviceConfigurationEntry.ServiceType.IsAssignableFrom (serviceConfigurationEntry.ImplementationType), Is.True);
      Assert.That (serviceConfigurationEntry.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_DiscoveryServiceReturnsArrayList ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Expect (stub => stub.GetTypes (null, false)).Return (
          new ArrayList(new[] { typeof(ITestSingletonConcreteImplementationAttributeType)}));

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub);

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.First ();
      Assert.That (serviceConfigurationEntry.ServiceType.IsAssignableFrom (serviceConfigurationEntry.ImplementationType), Is.True);
      Assert.That (serviceConfigurationEntry.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_ServiceHasNoConcreteImplementationAttributeDefined ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ICollection) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetDefaultConfiguration_ServiceHasConcreteImplementationAttributeDefined ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (1));
      var resultEntry = serviceConfigurationEntries.ToArray()[0];
      Assert.That (resultEntry.ServiceType, Is.EqualTo(typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (resultEntry.ImplementationType, Is.EqualTo(typeof (TestConcreteImplementationAttributeType)));
      Assert.That (resultEntry.Lifetime, Is.EqualTo(LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_UnitTestAssembly ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { Assembly.GetExecutingAssembly() });

      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (9)); //ServiceLocation.TestDomain services
    }
    
  }
}
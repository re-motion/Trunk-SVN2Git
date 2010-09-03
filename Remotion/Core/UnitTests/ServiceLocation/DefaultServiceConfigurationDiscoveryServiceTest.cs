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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using System.Linq;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceConfigurationDiscoveryServiceTest
  {
    // TODO Review 3164: Rename the tests to match the method names
    // TODO Review 3164: Reorder the tests to match the tested methods' order

    [Test]
    public void GetServiceConfigurationEntries_ServiceHasNoConcreteImplementationAttributeDefined ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ICollection) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetServiceConfigurationEntries_ServiceHasConcreteImplementationAttributeDefined ()
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
    public void GetServiceConfigurationEntries_UnitTestAssembly ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { Assembly.GetExecutingAssembly() });

      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (9)); //ServiceLocation.TestDomain services
    }
    
    [Test]
    public void GetDefaultConfiguration ()
    {
      // TODO Review 3164: Rewrite to use a stub instead of the real service
      var typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService ();
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryService);

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.Count (), Is.GreaterThan (0));
      foreach (var serviceConfigurationEntry in serviceConfigurationEntries)
      {
        if (!serviceConfigurationEntry.ImplementationType.Name.StartsWith ("Test"))
          Assert.That (serviceConfigurationEntry.ServiceType.IsAssignableFrom (serviceConfigurationEntry.ImplementationType), Is.True);

        Assert.That (
            serviceConfigurationEntry.Lifetime,
            Is.EqualTo (AttributeUtility.GetCustomAttribute<ConcreteImplementationAttribute> (serviceConfigurationEntry.ServiceType, false).Lifetime));
      }
    }
  }
}
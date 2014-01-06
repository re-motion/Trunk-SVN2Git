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
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceConfigurationDiscoveryServiceTest
  {
    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestSingletonConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos, 
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithMultipleConcreteImplementationAttributes ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesType) });

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub).ToArray ();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestMultipleConcreteImplementationAttributesType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos,
          Is.EqualTo (
              new[]
              {
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType2), LifetimeKind.Instance),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType3), LifetimeKind.Instance),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType1), LifetimeKind.Singleton),
              }));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicatePosition ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType) });

      Assert.That (
          () => DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType'. "
              + "Ambiguous ConcreteImplementationAttribute: Position must be unique.")
              .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicateImplementation ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType) });

      Assert.That (
          () => DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub).ToArray (),
          Throws.InvalidOperationException
            .With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType'. "
              + "Ambiguous ConcreteImplementationAttribute: Implementation type must be unique.")
            .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithNoConcreteImplementationAttribute ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { typeof (ICollection) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetDefaultConfiguration_Types ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var entry = serviceConfigurationEntries.Single();
      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          entry.ImplementationInfos,
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void GetDefaultConfiguration_Types_Unresolvable ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var entry = serviceConfigurationEntries.Single();
      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType)));
      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_Types_UnresolvableAndResolvable ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes) }).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var entry = serviceConfigurationEntries.Single();
      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes)));
      var implementationInfo = new ServiceImplementationInfo (
          typeof (TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting), LifetimeKind.Instance);
      Assert.That (entry.ImplementationInfos, Is.EqualTo (new[] { implementationInfo }));
    }

    [Test]
    public void GetDefaultConfiguration_Assembly ()
    {
      // Because the TestDomain contains test classes with ambiguous attributes, we expect an exception here.
      Assert.That (
          () => DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { GetType().Assembly }).ToArray(), 
            Throws.InvalidOperationException.With.Message.Contains ("Ambiguous"));
    }
  }
}
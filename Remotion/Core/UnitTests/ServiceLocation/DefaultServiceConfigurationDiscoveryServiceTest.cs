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
using NUnit.Framework;
using Remotion.ServiceLocation;
using System.Linq;
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
              + "Ambigious ConcreteImplementationAttribute: Position must be unique.")
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
              + "Ambigious ConcreteImplementationAttribute: Implementation type must be unique.")
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
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (1));
      var resultEntry = serviceConfigurationEntries.Single();
      Assert.That (resultEntry.ServiceType, Is.EqualTo(typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          resultEntry.ImplementationInfos,
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    [Ignore ("TODO 4652")]
    public void GetDefaultConfiguration_Assembly ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { GetType().Assembly });

// ReSharper disable PossibleNullReferenceException
      var testDomainEntries = serviceConfigurationEntries.Where (entry => entry.ServiceType.Namespace.StartsWith (GetType().Namespace + ".TestDomain"));
// ReSharper restore PossibleNullReferenceException
      Assert.That (testDomainEntries.Count(), Is.EqualTo (10));
    }
  }
}
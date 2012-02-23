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
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceConfigurationEntryTest
  {
    [Test]
    public void Initialize_WithSingleInfo ()
    {
      var implementationInfo = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), implementationInfo);

      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos, Is.EqualTo (new[] { implementationInfo }));
    }

    [Test]
    public void Initialize_WithAdditonalInfos ()
    {
      var implementationInfo1 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var implementationInfo2 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), implementationInfo1, implementationInfo2);

      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos, Is.EqualTo (new[] { implementationInfo1, implementationInfo2 }));
    }

    [Test]
    public void Initialize_WithEnumerable ()
    {
      var info1 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var info2 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      var entry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), new[] { info1, info2 });

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (entry.ImplementationInfos, Is.EqualTo (new[] { info1, info2 }));
    }

    [Test]
    public void Initialize_WithEnumerable_Empty ()
    {
      var entry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), new ServiceImplementationInfo[0]);
      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void CreateFromAttributes_Single ()
    {
      var attribute = new ConcreteImplementationAttribute (typeof (TestConcreteImplementationAttributeType)) { Lifetime = LifetimeKind.Singleton };

      var entry = ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestSingletonConcreteImplementationAttributeType), new[] { attribute });

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          entry.ImplementationInfos, 
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void CreateFromAttributes_Multiple ()
    {
      var attribute1 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1))
                       { Lifetime = LifetimeKind.Singleton };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType2))
                       { Lifetime = LifetimeKind.Instance };
      var attributes = new[] { attribute1, attribute2};

      var entry = ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestMultipleConcreteImplementationAttributesType), attributes);

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestMultipleConcreteImplementationAttributesType)));
      Assert.That (
          entry.ImplementationInfos,
          Is.EqualTo (
              new[]
              {
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType1), LifetimeKind.Singleton),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType2), LifetimeKind.Instance)
              }));
    }
  }
}
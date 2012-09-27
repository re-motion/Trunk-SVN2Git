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
using System.IO;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;

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
    public void Initialize_IncompatibleType ()
    {
      var implementationInfo = new ServiceImplementationInfo (typeof (object), LifetimeKind.Singleton);

      Assert.That (
          () => new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), implementationInfo),
          Throws.ArgumentException.With.Message.EqualTo (
              "The implementation type 'System.Object' does not implement the service type.\r\nParameter name: implementationInfos"));
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
                       { Lifetime = LifetimeKind.Singleton, Position = 0 };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType2))
                       { Lifetime = LifetimeKind.Instance, Position = 1 };
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

    [Test]
    public void CreateFromAttributes_Multiple_EntriesAreSortedCorrectly ()
    {
      var attribute1 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1))
                       { Lifetime = LifetimeKind.Singleton, Position = 0 };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType2))
                       { Lifetime = LifetimeKind.Instance, Position = -1 };
      var attributes = new[] { attribute1, attribute2 };

      var entry = ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestMultipleConcreteImplementationAttributesType), attributes);

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestMultipleConcreteImplementationAttributesType)));
      Assert.That (
          entry.ImplementationInfos,
          Is.EqualTo (
              new[]
              {
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType2), LifetimeKind.Instance),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType1), LifetimeKind.Singleton)
              }));
    }

    [Test]
    public void CreateFromAttributes_Multiple_WithEqualPositions_Throws ()
    {
      var attribute1 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1)) { Position = 1 };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType2)) { Position = 1 };
      var attributes = new[] { attribute1, attribute2 };

      Assert.That (
          () => ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestMultipleConcreteImplementationAttributesType), attributes),
          Throws.InvalidOperationException.With.Message.EqualTo ("Ambiguous ConcreteImplementationAttribute: Position must be unique."));
    }

    [Test]
    public void CreateFromAttributes_Multiple_WithEqualTypes_Throws ()
    {
      var attribute1 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1)) { Position = 1 };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1)) { Position = 2 };
      var attributes = new[] { attribute1, attribute2 };

      Assert.That (
          () => ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestMultipleConcreteImplementationAttributesType), attributes),
          Throws.InvalidOperationException.With.Message.EqualTo ("Ambiguous ConcreteImplementationAttribute: Implementation type must be unique."));
    }

    [Test]
    public void CreateFromAttributes_Multiple_WithEqualTypesAndPositions_ThrowsForType ()
    {
      var attribute1 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1)) { Position = 1 };
      var attribute2 = new ConcreteImplementationAttribute (typeof (TestMultipleConcreteImplementationAttributesType1)) { Position = 1 };
      var attributes = new[] { attribute1, attribute2 };

      Assert.That (
          () => ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestMultipleConcreteImplementationAttributesType), attributes),
          Throws.InvalidOperationException.With.Message.EqualTo ("Ambiguous ConcreteImplementationAttribute: Implementation type must be unique."));
    }

    [Test]
    public void CreateFromAttributes_IgnoreIfNotFoundFalse_InvalidAssembly ()
    {
      var attribute = new ConcreteImplementationAttribute ("DoesNotMatter, InvalidAssembly", ignoreIfNotFound: false);
      Assert.That (() => ServiceConfigurationEntry.CreateFromAttributes (typeof (object), new[] { attribute }), Throws.TypeOf<FileNotFoundException>());
    }

    [Test]
    public void CreateFromAttributes_IgnoreIfNotFoundTrue_InvalidAssembly ()
    {
      var attribute = new ConcreteImplementationAttribute ("DoesNotMatter, InvalidAssembly", ignoreIfNotFound: true);

      var entry = ServiceConfigurationEntry.CreateFromAttributes (typeof (object), new[] { attribute });

      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void CreateFromAttributes_IgnoreIfNotFoundFalse_InvalidType ()
    {
      var attribute = new ConcreteImplementationAttribute ("IDoNotExist, mscorlib", ignoreIfNotFound: false);
      Assert.That (() => ServiceConfigurationEntry.CreateFromAttributes (typeof (object), new[] { attribute }), Throws.TypeOf<TypeLoadException> ());
    }

    [Test]
    public void CreateFromAttributes_IgnoreIfNotFoundTrue_InvalidType ()
    {
      var attribute = new ConcreteImplementationAttribute ("IDoNotExist, mscorlib", ignoreIfNotFound: true);

      var entry = ServiceConfigurationEntry.CreateFromAttributes (typeof (object), new[] { attribute });

      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void CreateFromAttributes_IncompatibleType ()
    {
      var attribute = new ConcreteImplementationAttribute (typeof (object));

      Assert.That (
          () => ServiceConfigurationEntry.CreateFromAttributes (typeof (ITestSingletonConcreteImplementationAttributeType), new[] { attribute }),
          Throws.InvalidOperationException.With.Message.EqualTo ("The implementation type 'System.Object' does not implement the service type."));
    }

    [Test]
    public void ToString_EmptyEntry ()
    {
      var entry = new ServiceConfigurationEntry (typeof (IServiceProvider));

      Assert.That (entry.ToString (), Is.EqualTo ("System.IServiceProvider implementations: []"));
    }

    [Test]
    public void ToString_TwoEntries ()
    {
      var entry = new ServiceConfigurationEntry (
          typeof (IComparable),
          new ServiceImplementationInfo (typeof (int), LifetimeKind.Singleton),
          new ServiceImplementationInfo (typeof (string), LifetimeKind.Instance));

      Assert.That (entry.ToString (), Is.EqualTo (
        "System.IComparable implementations: [{System.Int32, Singleton}, {System.String, Instance}]"));
    }
  }
}
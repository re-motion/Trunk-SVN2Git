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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyWithStorageClass : BaseTest
  {
    [Test]
    public void StorageClass_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("NoAttribute");
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void StorageClass_WithPersistentAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Persistent");
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void StorageClass_WithTransactionAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Transaction");
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Transaction));
    }

    [Test]
    public void StorageClass_WithNoneAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("None");
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.None));
    }

    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata ();
      actual.SetStorageProperty (new SimpleColumnDefinition ("NoAttribute", typeof (string), "varchar", true));

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.AreEqual ("NoAttribute", StorageModelTestHelper.GetColumnName(actual));
    }

    [Test]
    public void GetMetadata_WithStorageClassPersistent ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Persistent");

      PropertyDefinition actual = propertyReflector.GetMetadata ();
      actual.SetStorageProperty (new SimpleColumnDefinition ("Persistent", typeof (string), "varchar", true));

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.Persistent",
          actual.PropertyName);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.AreEqual ("Persistent", StorageModelTestHelper.GetColumnName(actual));
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_DoesntThrow ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Transaction");
      propertyReflector.GetMetadata ();
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_SetsStorageClass ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Transaction");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      Assert.That (propertyDefinition.StorageClass, Is.EqualTo (StorageClass.Transaction));
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_NonPersistableDataType ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("TransactionWithDateTimeDataType");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      Assert.That (propertyDefinition.StorageClass, Is.EqualTo (StorageClass.Transaction));
      Assert.That (propertyDefinition.PropertyType, Is.EqualTo (typeof (DateTime)));
    }
  }
}

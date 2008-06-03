/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyWithStorageClass : BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.NoAttribute",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageClassPersistent ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Persistent");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.Persistent",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("Persistent", actual.StorageSpecificName);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Only StorageClass.Persistent is supported.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests.PropertyWithStorageClass, "
        + "property: Transaction")]
    public void GetMetadata_WithStorageClassTransaction ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<PropertyWithStorageClass> ("Transaction");

      propertyReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Only StorageClass.Persistent is supported.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute, "
        + "property: None")]
    public void GetMetadata_WithStorageClassNone ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("None");

      propertyReflector.GetMetadata ();
    }

    [StorageClass (StorageClass.Transaction)]
    public object Transaction
    {
      get { return null; }
      set { }
    }
  }
}

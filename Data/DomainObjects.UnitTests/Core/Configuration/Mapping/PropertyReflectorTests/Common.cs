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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class Common: BaseTest
  {
    [Test]
    public void GetMetadata_ForSingleProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (ReflectionBasedPropertyDefinition), actual);
      Assert.IsNotNull (actual);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreEqual ("Boolean", actual.StorageSpecificName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (bool), actual.PropertyType);
    }
  }
}

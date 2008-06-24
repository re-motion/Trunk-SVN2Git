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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common: StandardMappingTest
  {
    [Test]
    public void CreateRelationEndPointReflector()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      Assert.IsInstanceOfType (
          typeof (RdbmsRelationEndPointReflector), 
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties)), propertyInfo, Configuration.NameResolver));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NoAttribute");
      RelationEndPointReflector relationEndPointReflector =
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition (typeof (ClassWithManySideRelationProperties)), propertyInfo, Configuration.NameResolver);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithCollectionPropertyAndWithoutAttribute ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
        "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation", true, false);

      PropertyInfo propertyInfo = type.GetProperty ("LeftSide");
      RelationEndPointReflector relationEndPointReflector =
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.MandatoryAttribute' may be only applied to properties assignable to types "
        + "'Remotion.Data.DomainObjects.DomainObject' or 'Remotion.Data.DomainObjects.ObjectList`1[T]'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties();

      PropertyInfo propertyInfo = type.GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector =
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: PropertyWithStringAttribute")]
    public void GetMetadata_WithStringAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties ();
      PropertyInfo propertyInfo = type.GetProperty ("PropertyWithStringAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector =
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.BinaryPropertyAttribute' may be only applied to properties of type 'System.Byte[]'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: PropertyWithBinaryAttribute")]
    public void GetMetadata_WithBinaryAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties ();
      PropertyInfo propertyInfo = type.GetProperty ("PropertyWithBinaryAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector =
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "The classDefinition's class type 'Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.BaseClass' is not assignable "
        + "to the property's declaring type.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      Type classType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.BaseClass", true, false);
      Type declaringType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute", true, false);
      PropertyInfo propertyInfo = declaringType.GetProperty ("Int32");

      new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (classType), propertyInfo, Configuration.NameResolver);
    }

    private Type GetClassWithInvalidProperties ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties", true, false);
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> ());
    }
  }
}

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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
         "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NoAttribute",
         actual.PropertyName);
      Assert.IsFalse (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NotNullable");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NotNullable",
          actual.PropertyName);
      Assert.IsTrue (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      Assert.AreEqual ("The Sort Expression", relationEndPointDefinition.SortExpression);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: CollectionPropertyContainsKeyLeftSide")]
    public void GetMetadata_WithCollectionPropertyContainingTheKey ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("CollectionPropertyContainsKeyLeftSide");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only relation end points with a property type of 'Remotion.Data.DomainObjects.ObjectList`1[T]' can have a sort expression.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NonCollectionPropertyHavingASortExpressionLeftSide")]
    public void GetMetadata_WithNonCollectionPropertyHavingASortExpression ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("NonCollectionPropertyHavingASortExpressionLeftSide");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo, Configuration.NameResolver);

      relationEndPointReflector.GetMetadata ();
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> ());
    }
  }
}

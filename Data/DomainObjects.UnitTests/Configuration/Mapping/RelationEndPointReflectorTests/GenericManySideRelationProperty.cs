using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class GenericManySideRelationProperty : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithManySideRelationProperties",
          "ClosedGenericClassWithManySideRelationProperties",
          "TestDomain",
          typeof (ClosedGenericClassWithManySideRelationProperties),
          false, new List<Type> ());
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseUnidirectional"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseBidirectionalOneToOne"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreSame (GetPropertyDefinition ("BaseBidirectionalOneToMany"), relationEndPointDefinition.PropertyDefinition);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }


    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (string propertyName)
    {
      PropertyReflector propertyReflector = CreatePropertyReflector (propertyName);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      return new RdbmsRelationEndPointReflector (_classDefinition, propertyReflector.PropertyInfo);
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      Type type = typeof (ClosedGenericClassWithManySideRelationProperties);
      PropertyInfo propertyInfo = type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      return new PropertyReflector (_classDefinition, propertyInfo);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[
          string.Format ("{0}.{1}", typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName, propertyName)];
    }
  }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class RelationPropertyFinderTest
  {
    [Test]
    public void Initialize ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (DerivedClassWithMixedProperties), true, new List<Type> ());

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (DerivedClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithMixedProperties), true, new List<Type> ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithMixedProperties))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithOneSideRelationProperties), true, new List<Type> ());

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToMany"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToMany"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NoAttribute"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NotNullable"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToMany")
                  }));
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      PropertyInfo propertyInfo =
          type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That (propertyInfo, Is.Not.Null, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type>());
    }
  }
}
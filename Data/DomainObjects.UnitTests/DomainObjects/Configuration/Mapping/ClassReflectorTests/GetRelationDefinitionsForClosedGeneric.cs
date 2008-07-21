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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetRelationDefintionsForClosedGeneric : TestBase
  {
    private RelationDefinitionChecker _relationDefinitionChecker;
    private RelationDefinitionCollection _relationDefinitions;
    private ClassDefinitionCollection _classDefinitions;
    private ClassDefinition _closedGenericClassWithManySideRelationPropertiesClassDefinition;
    private ClassDefinition _closedGenericClassWithOneSideRelationPropertiesClassDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _relationDefinitionChecker = new RelationDefinitionChecker();
      _relationDefinitions = new RelationDefinitionCollection();
      _closedGenericClassWithManySideRelationPropertiesClassDefinition = CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition ();
      _closedGenericClassWithOneSideRelationPropertiesClassDefinition = CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition ();
      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_closedGenericClassWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetRelationDefinitions_ForManySide ()
    {
      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection ();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition ());

      ClassReflector classReflector = new ClassReflector (typeof (ClosedGenericClassWithManySideRelationProperties), Configuration.NameResolver);

      List<RelationDefinition> actualList = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
      Assert.That (actualList, Is.EqualTo (_relationDefinitions));
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClosedGenericClassWithOneSideRelationProperties), Configuration.NameResolver);

      List<RelationDefinition> actual = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Count);
    }


    private ClassDefinition CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithOneSideRelationProperties",
          "ClosedGenericClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClosedGenericClassWithOneSideRelationProperties),
          false, new List<Type> ());

      return classDefinition;
    }

    private ClassDefinition CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithManySideRelationProperties",
          "ClosedGenericClassWithManySideRelationProperties",
          c_testDomainProviderID,
          typeof (ClosedGenericClassWithManySideRelationProperties),
          false, new List<Type> ());

      CreatePropertyDefinitionsForClosedGenericClassWithManySideRelationProperties (classDefinition);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClosedGenericClassWithManySideRelationProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseUnidirectional", "BaseUnidirectionalID", typeof (ObjectID), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne", "BaseBidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany", "BaseBidirectionalOneToManyID", typeof (ObjectID), true, null, StorageClass.Persistent));

    }

    private RelationDefinition CreateBaseUnidirectionalRelationDefinition ()
    {
      return new RelationDefinition (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClosedGenericClassWithManySideRelationProperties.BaseUnidirectional",
          CreateRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseUnidirectional",
              false),
          CreateAnonymousRelationEndPointDefinition ());
    }

    private RelationDefinition CreateBaseBidirectionalOneToOneRelationDefinition ()
    {
      return new RelationDefinition (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClosedGenericClassWithManySideRelationProperties.BaseBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithOneSideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBaseBidirectionalOneToManyRelationDefinition ()
    {
      return new RelationDefinition (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClosedGenericClassWithManySideRelationProperties.BaseBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithOneSideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
              false, "The Sort Expression"));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (string propertyName, bool isMandatory)
    {
      return new RelationEndPointDefinition (_closedGenericClassWithManySideRelationPropertiesClassDefinition, propertyName, isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForManySide (string propertyName, bool isMandatory)
    {
      return ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(_closedGenericClassWithOneSideRelationPropertiesClassDefinition, propertyName, isMandatory, CardinalityType.One, typeof (ClosedGenericClassWithManySideRelationProperties));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (string propertyName, bool isMandatory, string sortExpression)
    {
      return ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(_closedGenericClassWithOneSideRelationPropertiesClassDefinition, propertyName, isMandatory, CardinalityType.Many, typeof (ObjectList<ClosedGenericClassWithManySideRelationProperties>), sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition ()
    {
      return new AnonymousRelationEndPointDefinition (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }
  }
}

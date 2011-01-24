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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ClassReflectorForRelationsTests
{
  [TestFixture]
  public class GetRelationDefinitions : TestBase
  {
    private RelationDefinitionChecker _relationDefinitionChecker;
    private RelationDefinitionCollection _relationDefinitions;
    private ClassDefinitionCollection _classDefinitions;
    private ClassDefinition _classWithRealRelationEndPointsClassDefinition;
    private ClassDefinition _classWithVirtualRelationEndPointsClassDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _relationDefinitionChecker = new RelationDefinitionChecker();
      _relationDefinitions = new RelationDefinitionCollection();
      _classWithRealRelationEndPointsClassDefinition = CreateClassWithRealRelationEndPointsClassDefinition();
      _classWithVirtualRelationEndPointsClassDefinition = CreateClassWithVirtualRelationEndPointsClassDefinition();
      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_classWithRealRelationEndPointsClassDefinition);
      _classDefinitions.Add (_classWithVirtualRelationEndPointsClassDefinition);
    }

    // TODO 3560: Add ordinary unit tests using a factory stub

    [Test]
    public void GetRelationDefinitions_ForManySide ()
    {
      var classReflector = new ClassReflectorForRelations (typeof (ClassWithRealRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);
      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection ();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateNoAttributeRelationDefinition ());
      expectedDefinitions.Add (CreateNotNullableRelationDefinition ());
      expectedDefinitions.Add (CreateUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToManyRelationDefinition ());

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide ()
    {
      var classReflector = new ClassReflectorForRelations (typeof (ClassWithVirtualRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);

      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection ();
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateNoAttributeRelationDefinition ());
      expectedDefinitions.Add (CreateNotNullableRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToManyRelationDefinition ());

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
    }

    [Test]
    public void GetRelationDefinitions_ForBothSides ()
    {
      //get relation definitions for the many side
      var classReflector = new ClassReflectorForRelations (typeof (ClassWithRealRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);
      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      //get relation definitions for the one side
      classReflector = new ClassReflectorForRelations (typeof (ClassWithVirtualRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);
      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection ();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToManyRelationDefinition ());
      expectedDefinitions.Add (CreateNoAttributeRelationDefinition ());
      expectedDefinitions.Add (CreateNotNullableRelationDefinition ());
      expectedDefinitions.Add (CreateUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBidirectionalOneToManyRelationDefinition ());

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
     
      Assert.That (_classWithRealRelationEndPointsClassDefinition.MyRelationEndPointDefinitions.Count, Is.EqualTo (11));
      Assert.That (_classWithVirtualRelationEndPointsClassDefinition.MyRelationEndPointDefinitions.Count, Is.EqualTo (8));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints'.",
        MatchType = MessageMatch.Contains)]
    public void GetRelationDefinitions_WithMissingClassDefinition ()
    {
      var classReflector = new ClassReflectorForRelations (typeof (ClassWithRealRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);
      classReflector.GetRelationDefinitions (new ClassDefinitionCollection(), _relationDefinitions);
    }

    private ClassDefinition CreateClassWithVirtualRelationEndPointsClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithVirtualRelationEndPoints",
          "ClassWithVirtualRelationEndPoints",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithVirtualRelationEndPoints),
          false);
      classDefinition.SetRelationEndPointDefinitions (MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithRealRelationEndPointsClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithRealRelationEndPoints",
          "ClassWithRealRelationEndPoints",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithRealRelationEndPoints),
          false);

      CreatePropertyDefinitionsForClassWithRealRelationEndPoints (classDefinition);
      classDefinition.SetRelationEndPointDefinitions (MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition));

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithRealRelationEndPoints (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BaseUnidirectional", "BaseUnidirectionalID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BaseBidirectionalOneToOne", "BaseBidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BaseBidirectionalOneToMany", "BaseBidirectionalOneToManyID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BasePrivateUnidirectional", "BasePrivateUnidirectionalID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BasePrivateBidirectionalOneToOne", "BasePrivateBidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPointsNotInMapping), "BasePrivateBidirectionalOneToMany", "BasePrivateBidirectionalOneToManyID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPoints), "NoAttribute", "NoAttributeID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPoints), "NotNullable", "NotNullableID", typeof (ObjectID), false, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPoints), "Unidirectional", "UnidirectionalID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne", "BidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany", "BidirectionalOneToManyID", typeof (ObjectID), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private RelationDefinition CreateNoAttributeRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NoAttribute->"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttribute",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NoAttribute", 
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttribute",
              false, null));
    }

    private RelationDefinition CreateNotNullableRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NotNullable->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullable",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NotNullable",
              true),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullable",
              true, null));
    }

    private RelationDefinition CreateBaseUnidirectionalRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseUnidirectional",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseUnidirectional", 
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBasePrivateUnidirectionalRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateUnidirectional",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateUnidirectional",
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateUnidirectionalRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.Unidirectional",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.Unidirectional", false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBaseBidirectionalOneToOneRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"+
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseBidirectionalOneToOne->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBasePrivateBidirectionalOneToOneRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateBidirectionalOneToOne->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBidirectionalOneToOneRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.BidirectionalOneToOne->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.BidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBaseBidirectionalOneToManyRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseBidirectionalOneToMany->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BaseBidirectionalOneToMany", 
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
              false, "NoAttribute"));
    }

    private RelationDefinition CreateBasePrivateBidirectionalOneToManyRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"+
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateBidirectionalOneToMany->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPointsNotInMapping.BasePrivateBidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
              false, "NoAttribute"));
    }

    private RelationDefinition CreateBidirectionalOneToManyRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.BidirectionalOneToMany->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.BidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany",
              false, "NoAttribute"));
    }

    private RelationDefinition CreateExpectedRelationDefinition (string id, IRelationEndPointDefinition endPointDefinition1, IRelationEndPointDefinition endPointDefinition2)
    {
      var relationDefinition = new RelationDefinition (id, endPointDefinition1, endPointDefinition2);
      endPointDefinition1.SetRelationDefinition (relationDefinition);
      endPointDefinition2.SetRelationDefinition (relationDefinition);
      return relationDefinition;
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (string propertyName, bool isMandatory)
    {
      return new RelationEndPointDefinition (_classWithRealRelationEndPointsClassDefinition, propertyName, isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForManySide (string propertyName, bool isMandatory)
    {
      return ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(_classWithVirtualRelationEndPointsClassDefinition, propertyName, isMandatory, CardinalityType.One, typeof (ClassWithRealRelationEndPoints));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (string propertyName, bool isMandatory, string sortExpression)
    {
      return ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(_classWithVirtualRelationEndPointsClassDefinition, propertyName, isMandatory, CardinalityType.Many, typeof (ObjectList<ClassWithRealRelationEndPoints>), sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition ()
    {
      return new AnonymousRelationEndPointDefinition (_classWithVirtualRelationEndPointsClassDefinition);
    }
  }
}

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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ClassReflectorForRelationsTests
{
  [TestFixture]
  public class GetRelationDefinitionsForClosedGeneric : TestBase
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
      _closedGenericClassWithManySideRelationPropertiesClassDefinition = CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition();
      _closedGenericClassWithOneSideRelationPropertiesClassDefinition = CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition();
      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_closedGenericClassWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetRelationDefinitions_ForManySide ()
    {
      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition());

      var classReflector = new ClassReflectorForRelations (
          typeof (ClosedGenericClassWithRealRelationEndPoints), Configuration.NameResolver);

      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide ()
    {
      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection();
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition());

      var classReflector = new ClassReflectorForRelations (
          typeof (ClosedGenericClassWithVirtualRelationEndPoints), Configuration.NameResolver);

      classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
    }


    private ClassDefinition CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClosedGenericClassWithVirtualRelationEndPoints",
              "ClosedGenericClassWithVirtualRelationEndPoints",
              TestDomainStorageProviderDefinition,
              typeof (ClosedGenericClassWithVirtualRelationEndPoints),
              false);

      classDefinition.SetPropertyDefinitions (
          MappingObjectFactory.CreatePropertyDefinitionCollection (classDefinition, GetPropertyInfos (classDefinition)));
      classDefinition.SetRelationEndPointDefinitions (MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition));

      return classDefinition;
    }

    private ClassDefinition CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClosedGenericClassWithManySideRelationProperties",
              "ClosedGenericClassWithManySideRelationProperties",
              TestDomainStorageProviderDefinition,
              typeof (ClosedGenericClassWithRealRelationEndPoints),
              false);

      classDefinition.SetPropertyDefinitions (
          MappingObjectFactory.CreatePropertyDefinitionCollection (classDefinition, GetPropertyInfos (classDefinition)));
      classDefinition.SetRelationEndPointDefinitions (MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition));

      return classDefinition;
    }

    private RelationDefinition CreateBaseUnidirectionalRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClosedGenericClassWithRealRelationEndPoints:" +
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseUnidirectional",
          CreateRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseUnidirectional",
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBaseBidirectionalOneToOneRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClosedGenericClassWithRealRelationEndPoints:"
          +
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseBidirectionalOneToOne->"
          +
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithVirtualRelationEndPointsNotInMapping`1.BaseBidirectionalOneToOne",
          CreateRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseBidirectionalOneToOne",
              false),
          CreateVirtualRelationEndPointDefinitionForManySide (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithVirtualRelationEndPointsNotInMapping`1.BaseBidirectionalOneToOne",
              false));
    }

    private RelationDefinition CreateBaseBidirectionalOneToManyRelationDefinition ()
    {
      return CreateExpectedRelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClosedGenericClassWithRealRelationEndPoints:"
          +
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseBidirectionalOneToMany->"
          +
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithVirtualRelationEndPointsNotInMapping`1.BaseBidirectionalOneToMany",
          CreateRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithRealRelationEndPointsNotInMapping`1.BaseBidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.GenericClassWithVirtualRelationEndPointsNotInMapping`1.BaseBidirectionalOneToMany",
              false,
              "BaseUnidirectional"));
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
      return new RelationEndPointDefinition (_closedGenericClassWithManySideRelationPropertiesClassDefinition[propertyName], isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForManySide (string propertyName, bool isMandatory)
    {
      return
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              _closedGenericClassWithOneSideRelationPropertiesClassDefinition,
              propertyName,
              isMandatory,
              CardinalityType.One,
              typeof (ClosedGenericClassWithRealRelationEndPoints));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (
        string propertyName, bool isMandatory, string sortExpression)
    {
      return
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              _closedGenericClassWithOneSideRelationPropertiesClassDefinition,
              propertyName,
              isMandatory,
              CardinalityType.Many,
              typeof (ObjectList<ClosedGenericClassWithRealRelationEndPoints>),
              sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition ()
    {
      return new AnonymousRelationEndPointDefinition (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }

    private PropertyInfo[] GetPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder (
          classDefinition.ClassType,
          classDefinition,
          classDefinition.BaseClass == null,
          true,
          new ReflectionBasedNameResolver (),
          classDefinition.PersistentMixinFinder);
      return propertyFinder.FindPropertyInfos ();
    }
  }
}
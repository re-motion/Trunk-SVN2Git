// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests
{
  public abstract class MappingReflectionIntegrationTestBase
  {
    private static readonly IEnumerable<Type> s_domainObjectTypes = 
        typeof (MappingReflectionTestBase).Assembly.GetTypes ().Where (t => typeof (DomainObject).IsAssignableFrom (t)).ToArray ();

    private MappingReflector _mappingReflector;
    private IDictionary<Type, ClassDefinition> _classDefinitions;
    private RelationDefinition[] _relationDefinitions;

    [SetUp]
    public virtual void SetUp ()
    {
      var reflectedTypes = ForTheseReflectedTypes().ToArray();
      _mappingReflector = CreateMappingReflector (reflectedTypes);

      _classDefinitions = _mappingReflector.GetClassDefinitions ().ToDictionary (cd => cd.ClassType);
      _mappingReflector.CreateClassDefinitionValidator ().Validate (_classDefinitions.Values);

      _relationDefinitions = _mappingReflector.GetRelationDefinitions (_classDefinitions);
      _mappingReflector.CreateRelationDefinitionValidator ().Validate (_relationDefinitions);
    }

    protected IDictionary<Type, ClassDefinition> ClassDefinitions
    {
      get { return _classDefinitions; }
    }

    protected virtual IEnumerable<Type> ForTheseReflectedTypes ()
    {
      return AllDomainObjectTypesFromThisNamespace ();
    }

    private IEnumerable<Type> AllDomainObjectTypesFromThisNamespace ()
    {
      return s_domainObjectTypes.Where (t => t.Namespace == GetType ().Namespace);
    }

    private MappingReflector CreateMappingReflector (Type[] reflectedTypes)
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (reflectedTypes);

      return MappingReflectorObjectMother.CreateMappingReflector (typeDiscoveryServiceStub);
    }

    protected IRelationEndPointDefinition GetRelationEndPointDefinition (ClassDefinition classDefinition, Type declaringType, string shortPropertyName)
    {
      return classDefinition.GetRelationEndPointDefinition (declaringType.FullName + "." + shortPropertyName);
    }
  }
}
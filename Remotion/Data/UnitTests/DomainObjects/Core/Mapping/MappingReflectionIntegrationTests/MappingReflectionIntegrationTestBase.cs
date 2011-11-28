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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests
{
  public abstract class MappingReflectionIntegrationTestBase
  {
    private static readonly IEnumerable<Type> s_domainObjectTypes = 
        typeof (MappingReflectionTestBase).Assembly.GetTypes ().Where (t => typeof (DomainObject).IsAssignableFrom (t)).ToArray ();

    private IDictionary<Type, ClassDefinition> _typeDefinitions;
    private MappingConfiguration _mappingConfiguration;

    [SetUp]
    public virtual void SetUp ()
    {
      var reflectedTypes = ForTheseReflectedTypes().ToArray();
      var mappingReflector = CreateMappingReflector (reflectedTypes);

      var storageGroupBasedStorageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (
          StandardConfiguration.Instance.GetPersistenceConfiguration ());
      var persistenceModelLoader = new PersistenceModelLoader (storageGroupBasedStorageProviderDefinitionFinder);
      _mappingConfiguration = new MappingConfiguration (mappingReflector, persistenceModelLoader);

      _typeDefinitions = _mappingConfiguration.GetTypeDefinitions().ToDictionary (cd => cd.ClassType);
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    protected IDictionary<Type, ClassDefinition> TypeDefinitions
    {
      get { return _typeDefinitions; }
    }

    protected virtual IEnumerable<Type> ForTheseReflectedTypes ()
    {
      return AllDomainObjectTypesFromThisNamespace ();
    }

    protected IRelationEndPointDefinition GetRelationEndPointDefinition (ClassDefinition classDefinition, Type declaringType, string shortPropertyName)
    {
      return classDefinition.GetRelationEndPointDefinition (declaringType.FullName + "." + shortPropertyName);
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
  }
}
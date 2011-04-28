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
using System.ComponentModel.Design;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class MappingReflector : IMappingLoader
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingReflector));
    private readonly IMappingNameResolver _nameResolver;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly ITypeDiscoveryService _typeDiscoveryService;

    // This ctor is required when the MappingReflector is instantiated as a configuration element from a config file.
    public MappingReflector () : this(ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService())
    {
      
    }

    public MappingReflector (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
      _nameResolver = new ReflectionBasedNameResolver ();
      _mappingObjectFactory = new ReflectionBasedMappingObjectFactory (_nameResolver);
    }

    public ITypeDiscoveryService TypeDiscoveryService
    {
      get { return _typeDiscoveryService; }
    }

    public ClassDefinition[] GetClassDefinitions ()
    {
      s_log.Info ("Reflecting class definitions...");

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to reflect class definitions: {elapsed}."))
      {
        var types = GetDomainObjectTypesSorted();
        var classDefinitions = MappingObjectFactory.CreateClassDefinitionCollection (types);

        return classDefinitions
            .LogAndReturn (s_log, LogLevel.Info, result => string.Format ("Generated {0} class definitions.", result.Length));
      }
    }

    public RelationDefinition[] GetRelationDefinitions (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      s_log.InfoFormat ("Reflecting relation definitions of {0} class definitions...", classDefinitions.Count);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to reflect relation definitions: {elapsed}."))
      {
        var relationDefinitions = MappingObjectFactory.CreateRelationDefinitionCollection (classDefinitions);
        return relationDefinitions
            .LogAndReturn (s_log, LogLevel.Info, result => string.Format ("Generated {0} relation definitions.", result.Length));
      }
    }

    private IEnumerable<Type> GetDomainObjectTypes ()
    {
      return (from type in _typeDiscoveryService.GetTypes (typeof (DomainObject), false).Cast<Type>()
              where !type.IsDefined (typeof (IgnoreForMappingConfigurationAttribute), false)
                //TODO COMMONS-825: test this
              && !ReflectionUtility.IsDomainObjectBase (type)
              select type).Distinct();
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      return GetDomainObjectTypes ().OrderBy (t => t.FullName, StringComparer.OrdinalIgnoreCase).ToArray ();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public IMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    public IClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator (
          new DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule (),
          new DomainObjectTypeIsNotGenericValidationRule (),
          new InheritanceHierarchyFollowsClassHierarchyValidationRule (),
          new StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule (),
          new ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule (),
          new StorageGroupTypesAreSameWithinInheritanceTreeRule ());
    }

    public IPropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator (
        new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule (),
        new MappingAttributesAreSupportedForPropertyTypeValidationRule (),
        new StorageClassIsSupportedValidationRule (),
        new PropertyTypeIsSupportedValidationRule ());
    }

    public IRelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator (
          new RdbmsRelationEndPointCombinationIsSupportedValidationRule (),
          new SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule (),
          new VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule (),
          new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule (),
          new ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule (),
          new RelationEndPointPropertyTypeIsSupportedValidationRule (),
          new RelationEndPointNamesAreConsistentValidationRule (),
          new RelationEndPointTypesAreConsistentValidationRule (),
          new CheckForInvalidRelationEndPointsValidationRule (),
          new CheckForTypeNotFoundClassDefinitionValidationRule ());
    }

    public ISortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator (new SortExpressionIsValidValidationRule ());
    }
  }
}

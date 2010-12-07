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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase : IMappingLoader
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (MappingReflectorBase));

    private readonly IMappingNameResolver _nameResolver = new ReflectionBasedNameResolver();

    protected abstract IEnumerable<Type> GetDomainObjectTypes ();

    public IEnumerable<ClassDefinition> GetClassDefinitions ()
    {
      s_log.Info ("Reflecting class definitions...");

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to reflect class definitions: {elapsed}."))
      {
        var types = GetDomainObjectTypesSorted ();

        // TODO 3554: Move to collection factory
        var inheritanceHierarchyFilter = new InheritanceHierarchyFilter (types);
        var leafTypes = inheritanceHierarchyFilter.GetLeafTypes ();

        var classDefinitions = new ClassDefinitionCollection ();
        var classReflectors = from domainObjectClass in leafTypes
                              select ClassReflector.CreateClassReflector (domainObjectClass, NameResolver);

        foreach (ClassReflector classReflector in classReflectors)
          classReflector.GetClassDefinition (classDefinitions);

        var classesByBaseClass = (from classDefinition in classDefinitions.Cast<ClassDefinition> ()
                                  where classDefinition.BaseClass!=null
                                  group classDefinition by classDefinition.BaseClass)
                                  .ToDictionary (grouping => grouping.Key, grouping => (IEnumerable<ClassDefinition>) grouping);

        foreach (ClassDefinition classDefinition in classDefinitions)
        {
          IEnumerable<ClassDefinition> derivedClasses;
          if (!classesByBaseClass.TryGetValue (classDefinition, out derivedClasses))
            derivedClasses = Enumerable.Empty<ClassDefinition>();
          
          classDefinition.SetDerivedClasses (new ClassDefinitionCollection (derivedClasses, true, true));
        }
        // TODO 3554: ... up to here
        
        return classDefinitions
            .LogAndReturn (s_log, LogLevel.Info, result => string.Format ("Generated {0} class definitions.", result.Count))
            .Cast<ClassDefinition>();
      }
    }

    public IEnumerable<RelationDefinition> GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      s_log.InfoFormat ("Reflecting relation definitions of {0} class definitions...", classDefinitions.Count);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to reflect relation definitions: {elapsed}."))
      {
        var relationDefinitions = new RelationDefinitionCollection();
        foreach (var classReflector in CreateClassReflectorsForRelations (classDefinitions))
          classReflector.GetRelationDefinitions (classDefinitions, relationDefinitions);

        return relationDefinitions
            .LogAndReturn (s_log, LogLevel.Info, result => string.Format ("Generated {0} relation definitions.", result.Count))
            .Cast<RelationDefinition>();
      }
    }

    private IEnumerable<ClassReflector> CreateClassReflectors ()
    {
      
    }

    private IEnumerable<ClassReflectorForRelations> CreateClassReflectorsForRelations (IEnumerable classDefinitions)
    {
      return from classDefinition in classDefinitions.Cast<ClassDefinition>()
             select ClassReflectorForRelations.CreateClassReflector (classDefinition.ClassType, NameResolver);
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      return GetDomainObjectTypes().OrderBy (t => t.FullName, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public ClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator (
          new DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule (),
          new DomainObjectTypeIsNotGenericValidationRule (),
          new InheritanceHierarchyFollowsClassHierarchyValidationRule (),
          new StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule (),
          new ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule (),
          new StorageGroupTypesAreSameWithinInheritanceTreeRule ());
    }

    public PropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator (
        new PropertyNamesAreUniqueWithinInheritanceTreeValidationRule (),
        new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule (),
        new MappingAttributesAreSupportedForPropertyTypeValidationRule (),
        new StorageClassIsSupportedValidationRule (),
        new PropertyTypeIsSupportedValidationRule ());
    }

    public RelationDefinitionValidator CreateRelationDefinitionValidator ()
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
          new CheckForPropertyNotFoundRelationEndPointsValidationRule (),
          new CheckForTypeNotFoundClassDefinitionValidationRule ());
    }

    public SortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator (new SortExpressionIsValidValidationRule ());
    }
  }
}
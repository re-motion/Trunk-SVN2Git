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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase: IMappingLoader
  {
    private readonly IMappingNameResolver _nameResolver = new ReflectionBasedNameResolver();

    protected abstract IEnumerable<Type> GetDomainObjectTypes();

    public ClassDefinitionCollection GetClassDefinitions()
    {
      var classDefinitions = new ClassDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectors())
        classReflector.GetClassDefinition (classDefinitions);

      return classDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      var relationDefinitions = new RelationDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectorsForRelations (classDefinitions))
        classReflector.GetRelationDefinitions (classDefinitions, relationDefinitions);

      return relationDefinitions;
    }

    private IEnumerable<ClassReflector> CreateClassReflectors()
    {
      var inheritanceHierarchyFilter = new InheritanceHierarchyFilter (GetDomainObjectTypesSorted());
      return from domainObjectClass in inheritanceHierarchyFilter.GetLeafTypes()
             select ClassReflector.CreateClassReflector (domainObjectClass, NameResolver);
    }

    private IEnumerable<ClassReflector> CreateClassReflectorsForRelations (IEnumerable classDefinitions)
    {
      return from classDefinition in classDefinitions.Cast<ClassDefinition>()
             select ClassReflector.CreateClassReflector (classDefinition.ClassType, NameResolver);
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      return GetDomainObjectTypes ().OrderBy (t => t.FullName, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }
  }
}

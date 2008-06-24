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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase: IMappingLoader
  {
    private readonly IMappingNameResolver _nameResolver = new ReflectionBasedNameResolver();

    protected MappingReflectorBase()
    {
    }

    protected abstract Type[] GetDomainObjectTypes();

    public ClassDefinitionCollection GetClassDefinitions()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectors())
        classReflector.GetClassDefinition (classDefinitions);

      return classDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectorsForRelations (classDefinitions))
        classReflector.GetRelationDefinitions (classDefinitions, relationDefinitions);

      return relationDefinitions;
    }

    private List<ClassReflector> CreateClassReflectors()
    {
      List<ClassReflector> classReflectors = new List<ClassReflector>();
      InheritanceHierarchyFilter inheritanceHierarchyFilter = new InheritanceHierarchyFilter (GetDomainObjectTypesSorted());
      foreach (Type domainObjectClass in inheritanceHierarchyFilter.GetLeafTypes())
        classReflectors.Add (ClassReflector.CreateClassReflector (domainObjectClass, NameResolver));
      
      return classReflectors;
    }

    private List<ClassReflector> CreateClassReflectorsForRelations (ClassDefinitionCollection classDefinitions)
    {
      List<ClassReflector> classReflectors = new List<ClassReflector> ();
      foreach (ReflectionBasedClassDefinition classDefinition in classDefinitions)
        classReflectors.Add (ClassReflector.CreateClassReflector (classDefinition.ClassType, NameResolver));

      return classReflectors;
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      Type[] domainObjectTypes = GetDomainObjectTypes ();

      Array.Sort (
          domainObjectTypes,
          delegate (Type left, Type right) { return string.Compare (left.FullName, right.FullName, StringComparison.OrdinalIgnoreCase); });

      return domainObjectTypes;
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

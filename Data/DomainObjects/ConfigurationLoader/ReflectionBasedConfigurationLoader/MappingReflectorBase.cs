// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

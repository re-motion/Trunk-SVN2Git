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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflectorForRelations"/> is used to build the <see cref="RelationDefinition"/> objects for a type.
  /// </summary>
  public class ClassReflectorForRelations
  {
    public static ClassReflectorForRelations CreateClassReflector (Type type, IMappingNameResolver nameResolver)
    {
      return new ClassReflectorForRelations (type, nameResolver);
    }

    public ClassReflectorForRelations (Type type, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      Type = type;
      NameResolver = nameResolver;
      PersistentMixinFinder = new PersistentMixinFinder (type, ReflectionUtility.IsInheritanceRoot (Type));
    }

    public PersistentMixinFinder PersistentMixinFinder { get; private set; }
    public Type Type { get; private set; }
    public IMappingNameResolver NameResolver { get; private set; }

    public List<RelationDefinition> GetRelationDefinitions (
        ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition> ();
      ReflectionBasedClassDefinition classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      foreach (PropertyInfo propertyInfo in GetRelationPropertyInfos (classDefinition, PersistentMixinFinder))
      {
        RelationReflector relationReflector = RelationReflector.CreateRelationReflector (classDefinition, propertyInfo, NameResolver);
        RelationDefinition relationDefinition = relationReflector.GetMetadata (classDefinitions, relationDefinitions);
        if (relationDefinition != null)
          relations.Add (relationDefinition);
        
        // RelationDefinition relationDefinition = relationReflector.GetMetadata (classDefinitions);
        // if (!relationDefs.Contains (relationDefinition)
        // {
        //   relationDefs.Add (relationDefinition);
        //   RelationReflector.AddRelationDefinitionToClassDefinitions (relationDefiniiton)
        // }
      }

      return relations;
    }

    private IEnumerable<PropertyInfo> GetRelationPropertyInfos (ReflectionBasedClassDefinition classDefinition, PersistentMixinFinder persistentMixinFinder)
    {
      RelationPropertyFinder relationPropertyFinder = new RelationPropertyFinder (Type, ReflectionUtility.IsInheritanceRoot (Type), NameResolver);
      return relationPropertyFinder.FindPropertyInfos (classDefinition);
    }
  }
}
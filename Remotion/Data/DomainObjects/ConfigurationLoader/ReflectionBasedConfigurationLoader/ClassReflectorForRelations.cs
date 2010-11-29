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
using System.Linq;
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
    }

    public Type Type { get; private set; }
    public IMappingNameResolver NameResolver { get; private set; }

    public void GetRelationDefinitions (ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      ReflectionBasedClassDefinition classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      var relationDefinitionsForClass = new RelationDefinitionCollection();
      foreach (PropertyInfo propertyInfo in GetRelationPropertyInfos (classDefinition))
      {
        RelationReflector relationReflector = new RelationReflector (
            classDefinition, propertyInfo, NameResolver, new ReflectionBasedRelationEndPointDefinitionFactory());
        RelationDefinition relationDefinition = relationReflector.GetMetadata (classDefinitions);
        if (!relationDefinitions.Contains (relationDefinition.ID))
          relationDefinitions.Add (relationDefinition);
        else
          relationDefinition = relationDefinitions[relationDefinition.ID];

        if (!relationDefinitionsForClass.Contains (relationDefinition))
          relationDefinitionsForClass.Add (relationDefinition);
      }
      classDefinition.SetRelationDefinitions (new RelationDefinitionCollection (relationDefinitionsForClass.Cast<RelationDefinition>(), true));
    }

    private IEnumerable<PropertyInfo> GetRelationPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      RelationPropertyFinder relationPropertyFinder = new RelationPropertyFinder (
          Type, ReflectionUtility.IsInheritanceRoot (Type), classDefinition.ClassType==Type, NameResolver, classDefinition.PersistentMixinFinder);
      return relationPropertyFinder.FindPropertyInfos();
    }
  }
}
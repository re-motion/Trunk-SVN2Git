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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflectorForRelations"/> is used to build the <see cref="RelationDefinition"/> objects for a type.
  /// </summary>
  public class ClassReflectorForRelations
  {
    public static ClassReflectorForRelations CreateClassReflector (Type type, IMappingObjectFactory mappingObjectFactory, IMappingNameResolver nameResolver)
    {
      return new ClassReflectorForRelations (type, mappingObjectFactory, nameResolver);
    }

    public ClassReflectorForRelations (Type type, IMappingObjectFactory mappingObjectFactory, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      Type = type;
      NameResolver = nameResolver;
      MappingObjectFactory = mappingObjectFactory;
    }

    public Type Type { get; private set; }
    public IMappingObjectFactory MappingObjectFactory { get; private set; }
    public IMappingNameResolver NameResolver { get; private set; }
    
    public void GetRelationDefinitions (ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      var classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      foreach (var endPoint in classDefinition.MyRelationEndPointDefinitions)
      {
        var relationReflector = new RelationReflector (classDefinition, endPoint.PropertyInfo, NameResolver);
        var relationDefinition = relationReflector.GetMetadata (classDefinitions);
        if (!relationDefinitions.Contains (relationDefinition.ID))
        {
          relationDefinition.EndPointDefinitions[0].SetRelationDefinition (relationDefinition);
          relationDefinition.EndPointDefinitions[1].SetRelationDefinition (relationDefinition);

          relationDefinitions.Add (relationDefinition);
        }
      }
    }
  }
}
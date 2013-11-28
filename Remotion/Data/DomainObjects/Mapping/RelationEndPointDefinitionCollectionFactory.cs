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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="RelationEndPointDefinitionCollectionFactory"/> is used to get a <see cref="RelationEndPointDefinitionCollection"/> for a 
  /// <see cref="ClassDefinition"/>
  /// </summary>
  public class RelationEndPointDefinitionCollectionFactory
  {
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMemberInfoNameResolver _memberInfoNameResolver;

    public RelationEndPointDefinitionCollectionFactory (IMappingObjectFactory mappingObjectFactory, IMemberInfoNameResolver memberInfoNameResolver)
    {
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull ("memberInfoNameResolver", memberInfoNameResolver);

      _mappingObjectFactory = mappingObjectFactory;
      _memberInfoNameResolver = memberInfoNameResolver;
    }

    public RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var endPoints = new RelationEndPointDefinitionCollection();
      foreach (var propertyInfo in GetRelationPropertyInfos(classDefinition))
      {
        var relationEndPoint = _mappingObjectFactory.CreateRelationEndPointDefinition (classDefinition, propertyInfo);
        endPoints.Add (relationEndPoint);
      }
      return endPoints;
    }

    private IEnumerable<IPropertyInformation> GetRelationPropertyInfos (ClassDefinition classDefinition)
    {
      var relationPropertyFinder = new RelationPropertyFinder (
          classDefinition.ClassType,
          classDefinition.BaseClass == null,
          true,
          _memberInfoNameResolver,
          classDefinition.PersistentMixinFinder);
      return relationPropertyFinder.FindPropertyInfos ();
    }
  }
}
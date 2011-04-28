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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="ReflectionBasedMappingObjectFactory"/> is used to create new mapping objects.
  /// </summary>
  public class ReflectionBasedMappingObjectFactory : IMappingObjectFactory
  {
    private readonly IMappingNameResolver _mappingNameResolver;

    public ReflectionBasedMappingObjectFactory (IMappingNameResolver mappingNameResolver)
    {
      ArgumentUtility.CheckNotNull ("mappingNameResolver", mappingNameResolver);

      _mappingNameResolver = mappingNameResolver;
    }

    public ClassDefinition CreateClassDefinition (Type type, ClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var classReflector = new ClassReflector (type, this, _mappingNameResolver);
      return classReflector.GetMetadata (baseClass);
    }

    public PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return new PropertyReflector (classDefinition, propertyInfo, _mappingNameResolver).GetMetadata();
    }

    public RelationDefinition CreateRelationDefinition (
        ClassDefinitionCollection classDefinitions, ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var relationReflector = new RelationReflector (classDefinition, propertyInfo, _mappingNameResolver);
      return relationReflector.GetMetadata (classDefinitions);
    }

    public IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, propertyInfo, _mappingNameResolver);
      return relationEndPointReflector.GetMetadata();
    }

    public ClassDefinitionCollection CreateClassDefinitionCollection (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var classDefinitionCollectionFactory = new ClassDefinitionCollectionFactory (this);
      return classDefinitionCollectionFactory.CreateClassDefinitionCollection (types);
    }

    public PropertyDefinitionCollection CreatePropertyDefinitionCollection (ClassDefinition classDefinition, IEnumerable<PropertyInfo> propertyInfos)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfos", propertyInfos);

      var factory = new PropertyDefinitionCollectionFactory (this);
      return factory.CreatePropertyDefinitions (classDefinition, propertyInfos);
    }

    public RelationDefinition[] CreateRelationDefinitionCollection (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var factory = new RelationDefinitionCollectionFactory (this);
      return factory.CreateRelationDefinitionCollection (classDefinitions);
    }

    public RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var factory = new RelationEndPointDefinitionCollectionFactory (this, _mappingNameResolver);
      return factory.CreateRelationEndPointDefinitionCollection (classDefinition);
    }
  }
}
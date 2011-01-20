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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="PropertyDefinitionCollectionFactory"/> is used to get a <see cref="PropertyDefinitionCollection"/> for a set of 
  /// <see cref="PropertyInfo"/>s.
  /// </summary>
  public class PropertyDefinitionCollectionFactory
  {
    private readonly IMappingNameResolver _mappingNameResolver;

    public PropertyDefinitionCollectionFactory (IMappingNameResolver mappingNameResolver)
    {
      ArgumentUtility.CheckNotNull ("mappingNameResolver", mappingNameResolver);

      _mappingNameResolver = mappingNameResolver;
    }

    public IMappingNameResolver MappingNameResolver
    {
      get { return _mappingNameResolver; }
    }

    public PropertyDefinitionCollection CreatePropertyDefinitions (
        ReflectionBasedClassDefinition classDefinition, IEnumerable<PropertyInfo> propertyInfos)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfos", propertyInfos);

      var propertyDefinitionsForClass = from PropertyInfo propertyInfo in propertyInfos
                                        select
                                            (PropertyDefinition)
                                            new PropertyReflector (classDefinition, propertyInfo, MappingNameResolver).GetMetadata();
      return new PropertyDefinitionCollection (propertyDefinitionsForClass, true);
    }
  }
}
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="RelationPropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a 
  /// <see cref="RelationEndPointDefinition"/>.
  /// </summary>
  public class RelationPropertyFinder : PropertyFinderBase
  {
    public RelationPropertyFinder (Type type, bool includeBaseProperties, IMappingNameResolver nameResolver)
        : base (type, includeBaseProperties, nameResolver)
    {
    }

    protected override bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (classDefinition, propertyInfo))
        return false;

      return IsRelationEndPoint (propertyInfo);
    }

    private bool IsRelationEndPoint (PropertyInfo propertyInfo)
    {
      return typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType) || ReflectionUtility.IsObjectList (propertyInfo.PropertyType);
    }
  }
}

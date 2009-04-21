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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="PropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a <see cref="PropertyDefinition"/>.
  /// </summary>
  public class PropertyFinder : PropertyFinderBase
  {
    public PropertyFinder (Type type, bool includeBaseProperties, IMappingNameResolver nameResolver)
        : base (type, includeBaseProperties, nameResolver)
    {
    }

    protected override bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (classDefinition, propertyInfo))
        return false;

      if (IsVirtualRelationEndPoint (classDefinition, propertyInfo))
        return false;

      return true;
    }

    private bool IsVirtualRelationEndPoint (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, propertyInfo, NameResolver);
      return relationEndPointReflector.IsVirtualEndRelationEndpoint ();
    }
  }
}

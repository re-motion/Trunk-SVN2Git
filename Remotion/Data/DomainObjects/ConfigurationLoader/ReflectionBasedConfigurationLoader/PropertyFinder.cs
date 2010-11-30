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
    private readonly ReflectionBasedClassDefinition _classDefinition;

    public PropertyFinder (
        Type type,
        ReflectionBasedClassDefinition classDefinition,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMappingNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder)
        : base (type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _classDefinition = classDefinition;
    }

    protected override bool FindPropertiesFilter (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (propertyInfo))
        return false;

      if (IsVirtualRelationEndPoint (propertyInfo))
        return false;

      return true;
    }

    protected override PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMappingNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder)
    {
      return new PropertyFinder (type, _classDefinition, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder);
    }

    private bool IsVirtualRelationEndPoint (PropertyInfo propertyInfo)
    {
      if (!ReflectionUtility.IsRelationType (propertyInfo.PropertyType))
        return false;
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (
          _classDefinition, propertyInfo, NameResolver);
      return relationEndPointReflector.IsVirtualEndRelationEndpoint();
    }
  }
}
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
  public class OppositePropertyFinder : PropertyFinderBase
  {
    private readonly string _oppositePropertyName;

    public OppositePropertyFinder (
        string oppositePropertyName,
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMappingNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder)
        : base (type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("oppositePropertyName", oppositePropertyName);

      _oppositePropertyName = oppositePropertyName;
    }

    protected override bool FindPropertiesFilter (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (propertyInfo))
        return false;

      return propertyInfo.Name == _oppositePropertyName;
    }

    protected override PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMappingNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder)
    {
      return new OppositePropertyFinder (
          _oppositePropertyName, type, includeBaseProperties, includeMixinProperties, nameResolver, persistentMixinFinder);
    }
  }
}
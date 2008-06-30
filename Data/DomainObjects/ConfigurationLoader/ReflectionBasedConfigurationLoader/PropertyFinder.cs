/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
    public PropertyFinder (Type type, bool includeBaseProperties, PersistentMixinFinder persistentMixinFinder, IMappingNameResolver nameResolver)
        : base (type, includeBaseProperties, persistentMixinFinder, nameResolver)
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

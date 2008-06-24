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
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase : MemberReflectorBase
  {
    private readonly BidirectionalRelationAttribute _bidirectionalRelationAttribute;
    private readonly ReflectionBasedClassDefinition _classDefinition;
    private readonly bool _isMixedProperty;

    protected RelationReflectorBase (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType, IMappingNameResolver nameResolver)
        : base (propertyInfo, nameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinitionType (classDefinition, propertyInfo);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      _classDefinition = classDefinition;
      _bidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (PropertyInfo, bidirectionalRelationAttributeType, true);
      _isMixedProperty = _classDefinition.HasPersistentMixin (PropertyInfo.DeclaringType);
    }

    public ReflectionBasedClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public BidirectionalRelationAttribute BidirectionalRelationAttribute
    {
      get { return _bidirectionalRelationAttribute; }
    }

    protected bool IsBidirectionalRelation
    {
      get { return _bidirectionalRelationAttribute != null; }
    }

    public bool IsMixedProperty
    {
      get { return _isMixedProperty; }
    }

    protected PropertyInfo GetOppositePropertyInfo ()
    {
      Type type = GetDomainObjectTypeFromRelationProperty (PropertyInfo);
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (type);
      if (oppositePropertyInfo != null)
        return oppositePropertyInfo;

      foreach (Type mixinType in PersistentMixinFinder.GetPersistentMixins (type))
      {
        oppositePropertyInfo = GetOppositePropertyInfo (mixinType);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
      }

      for (Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        oppositePropertyInfo = GetOppositePropertyInfo (baseType);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
      }

      throw CreateMappingException (
          null,
          PropertyInfo,
          "Opposite relation property '{0}' could not be found on type '{1}'.",
          BidirectionalRelationAttribute.OppositeProperty,
          GetDomainObjectTypeFromRelationProperty (PropertyInfo));
    }

    private PropertyInfo GetOppositePropertyInfo (Type type)
    {
      return type.GetProperty (BidirectionalRelationAttribute.OppositeProperty, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    protected Type GetDomainObjectTypeFromRelationProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (ReflectionUtility.IsObjectList (propertyInfo.PropertyType))
        return ReflectionUtility.GetObjectListTypeParameter (propertyInfo.PropertyType);
      else
        return propertyInfo.PropertyType;
    }

    private void CheckClassDefinitionType (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      if (!PropertyInfo.DeclaringType.IsAssignableFrom (classDefinition.ClassType) && !classDefinition.HasPersistentMixin (PropertyInfo.DeclaringType))
      {
        string message = string.Format (
            "The classDefinition's class type '{0}' is not assignable to the property's declaring type.\r\nDeclaring type: {1}, property: {2}",
            classDefinition.ClassType,
            propertyInfo.DeclaringType,
            propertyInfo.Name);
        throw new ArgumentTypeException (message, null, classDefinition.ClassType, propertyInfo.DeclaringType);
      }
    }
  }
}

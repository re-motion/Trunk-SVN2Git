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
    protected RelationReflectorBase (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType, IMappingNameResolver nameResolver)
        : base (propertyInfo, nameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      ClassDefinition = classDefinition;
      BidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (PropertyInfo, bidirectionalRelationAttributeType, true);
      DeclaringMixin = ClassDefinition.GetPersistentMixin (PropertyInfo.DeclaringType);
      DomainObjectTypeDeclaringProperty = GetDomainObjectTypeDeclaringProperty ();

      CheckClassDefinitionType ();
    }

    public ReflectionBasedClassDefinition ClassDefinition { get; private set; }
    public BidirectionalRelationAttribute BidirectionalRelationAttribute { get; private set; }
    public Type DeclaringMixin { get; private set; }

    public Type DomainObjectTypeDeclaringProperty { get; private set; }

    protected bool IsBidirectionalRelation
    {
      get { return BidirectionalRelationAttribute != null; }
    }

    public bool IsMixedProperty
    {
      get { return DeclaringMixin != null; }
    }

    private Type GetDomainObjectTypeDeclaringProperty ()
    {
      if (IsMixedProperty)
      {
#warning TODO: find the type where the mixin was first declared
        return ClassDefinition.ClassType;
      }
      else
        return PropertyInfo.DeclaringType;
    }

    protected PropertyInfo GetOppositePropertyInfo ()
    {
      Type type = GetDomainObjectTypeFromRelationProperty (PropertyInfo);
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (type);
      
      if (oppositePropertyInfo == null)
        oppositePropertyInfo = GetOppositePropertyInfoFromMixins (type);
      
      if (oppositePropertyInfo == null)
        oppositePropertyInfo = GetOppositePropertyInfoFromBaseTypes (type); // property defined on base type?

      if (oppositePropertyInfo == null)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' could not be found on type '{1}'.",
            BidirectionalRelationAttribute.OppositeProperty,
            GetDomainObjectTypeFromRelationProperty (PropertyInfo));
      }

      return oppositePropertyInfo;
    }

    private PropertyInfo GetOppositePropertyInfoFromMixins (Type type)
    {
      foreach (var mixinType in new PersistentMixinFinder (type).GetPersistentMixins ())
      {
        var oppositePropertyInfo = GetOppositePropertyInfo (mixinType);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
        else
          return GetOppositePropertyInfoFromBaseTypes (mixinType); // property defined on mixin's base type?
      }
      return null;
    }

    private PropertyInfo GetOppositePropertyInfoFromBaseTypes (Type type)
    {
      for (Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        var oppositePropertyInfo = GetOppositePropertyInfo (baseType);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
      }
      return null;
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

    private void CheckClassDefinitionType ()
    {
      if (!PropertyInfo.DeclaringType.IsAssignableFrom (ClassDefinition.ClassType) && !IsMixedProperty)
      {
        string message = string.Format (
            "The classDefinition's class type '{0}' is not assignable to the property's declaring type.\r\nDeclaring type: {1}, property: {2}",
            ClassDefinition.ClassType,
            PropertyInfo.DeclaringType,
            PropertyInfo.Name);
        throw new ArgumentTypeException (message, null, ClassDefinition.ClassType, PropertyInfo.DeclaringType);
      }
    }
  }
}

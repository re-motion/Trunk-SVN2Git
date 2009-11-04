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
using Remotion.Text;
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
      DeclaringDomainObjectTypeForProperty = GetDeclaringDomainObjectTypeForProperty ();

      CheckClassDefinitionType ();
    }

    public ReflectionBasedClassDefinition ClassDefinition { get; private set; }
    public BidirectionalRelationAttribute BidirectionalRelationAttribute { get; private set; }
    public Type DeclaringMixin { get; private set; }

    public Type DeclaringDomainObjectTypeForProperty { get; private set; }

    protected bool IsBidirectionalRelation
    {
      get { return BidirectionalRelationAttribute != null; }
    }

    public bool IsMixedProperty
    {
      get { return DeclaringMixin != null; }
    }

    private Type GetDeclaringDomainObjectTypeForProperty ()
    {
      if (IsMixedProperty)
      {
        var originalMixinTarget = ClassDefinition.PersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin);
        if (originalMixinTarget == null)
          throw new InvalidOperationException (string.Format ("IPersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin) evaluated and returned null."));
        return originalMixinTarget;
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
      foreach (var mixinType in new PersistentMixinFinder (type, true).GetPersistentMixins ())
      {
        var oppositePropertyInfo = GetOppositePropertyInfo (mixinType) ?? GetOppositePropertyInfoFromBaseTypes (mixinType);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
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

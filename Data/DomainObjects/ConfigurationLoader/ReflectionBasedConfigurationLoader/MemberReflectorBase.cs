// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the properties and relations of a class.</summary>
  public abstract class MemberReflectorBase
  {
    protected sealed class AttributeConstraint
    {
      private readonly Type[] _propertyTypes;
      private readonly string _message;

      public AttributeConstraint (string message, params Type[] propertyTypes)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("message", message);
        ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("propertyTypes", propertyTypes);

        _propertyTypes = propertyTypes;
        _message = message;
      }

      public Type[] PropertyTypes
      {
        get { return _propertyTypes; }
      }

      public string Message
      {
        get { return _message; }
      }
    }

    public const StorageClass DefaultStorageClass = StorageClass.Persistent;

    private Dictionary<Type, AttributeConstraint> _attributeConstraints = null;
    private PropertyInfo _propertyInfo;
    private readonly IMappingNameResolver _nameResolver;
    private StorageClassAttribute _storageClassAttribute;

    protected MemberReflectorBase (PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      _propertyInfo = propertyInfo;
      _nameResolver = nameResolver;
      _storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (PropertyInfo, true);
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public StorageClassAttribute StorageClassAttribute
    {
      get { return _storageClassAttribute; }
    }

    public StorageClass StorageClass
    {
      get { return StorageClassAttribute != null ? StorageClassAttribute.StorageClass : DefaultStorageClass; }
    }

    protected virtual void ValidatePropertyInfo()
    {
      CheckStorageClass();
      CheckSupportedPropertyAttributes();
    }

    protected virtual void AddAttributeConstraints (Dictionary<Type, AttributeConstraint> attributeConstraints)
    {
      ArgumentUtility.CheckNotNull ("attributeConstraints", attributeConstraints);

      attributeConstraints.Add (typeof (StringPropertyAttribute), CreateAttributeConstraintForValueTypeProperty<StringPropertyAttribute, string>());
      attributeConstraints.Add (typeof (BinaryPropertyAttribute), CreateAttributeConstraintForValueTypeProperty<BinaryPropertyAttribute, byte[]>());
      attributeConstraints.Add (typeof (MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute>());
    }

    protected AttributeConstraint CreateAttributeConstraintForValueTypeProperty<TAttribute, TProperty>()
        where TAttribute: Attribute
    {
      return new AttributeConstraint (
          string.Format ("The '{0}' may be only applied to properties of type '{1}'.", typeof (TAttribute).FullName, typeof (TProperty).FullName),
          typeof (TProperty));
    }

    protected AttributeConstraint CreateAttributeConstraintForRelationProperty<TAttribute>()
        where TAttribute: Attribute
    {
      return new AttributeConstraint (
          string.Format (
              "The '{0}' may be only applied to properties assignable to types '{1}' or '{2}'.",
              typeof (TAttribute),
              typeof (DomainObject),
              typeof (ObjectList<>)),
          typeof (DomainObject),
          typeof (ObjectList<>));
    }

    protected Dictionary<Type, AttributeConstraint> AttributeConstraints
    {
      get
      {
        if (_attributeConstraints == null)
        {
          _attributeConstraints = new Dictionary<Type, AttributeConstraint>();
          AddAttributeConstraints (_attributeConstraints);
        }
        return _attributeConstraints;
      }
    }

    private void CheckStorageClass()
    {
      if (_storageClassAttribute != null && _storageClassAttribute.StorageClass != StorageClass.Persistent && _storageClassAttribute.StorageClass != StorageClass.Transaction)
        throw CreateMappingException (null, PropertyInfo, "Only StorageClass.Persistent is supported.");
    }

    private void CheckSupportedPropertyAttributes()
    {
      foreach (Attribute attribute in AttributeUtility.GetCustomAttributes<Attribute> (PropertyInfo, true))
      {
        AttributeConstraint constraint;
        if (AttributeConstraints.TryGetValue (attribute.GetType(), out constraint))
        {
          if (!Array.Exists (constraint.PropertyTypes, IsPropertyTypeSupported))
            throw CreateMappingException (null, PropertyInfo, constraint.Message);
        }
      }
    }

    private bool IsPropertyTypeSupported (Type type)
    {
      if (type == typeof (ObjectList<>))
        return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType);
      return type.IsAssignableFrom (PropertyInfo.PropertyType);
    }

    protected virtual string GetPropertyName ()
    {
      return _nameResolver.GetPropertyName (PropertyInfo);
    }

    protected bool IsNullableFromAttribute ()
    {
      INullablePropertyAttribute attribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.IsNullable;
      return true;
    }

    protected MappingException CreateMappingException (Exception innerException, PropertyInfo propertyInfo, string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      StringBuilder messageBuilder = new StringBuilder();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("Declaring type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name);

      return new MappingException (messageBuilder.ToString(), innerException);
    }
  }
}

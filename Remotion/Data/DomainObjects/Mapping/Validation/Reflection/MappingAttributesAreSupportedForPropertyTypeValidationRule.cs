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
using System.Text;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that all applied attribute types of a property are supported.
  /// </summary>
  public class MappingAttributesAreSupportedForPropertyTypeValidationRule : IPropertyDefinitionValidationRule
  {
    private sealed class AttributeConstraint
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

    private Dictionary<Type, AttributeConstraint> _attributeConstraints;

    private Dictionary<Type, AttributeConstraint> AttributeConstraints
    {
      get
      {
        if (_attributeConstraints == null)
        {
          _attributeConstraints = new Dictionary<Type, AttributeConstraint> ();
          AddAttributeConstraints (_attributeConstraints);
        }
        return _attributeConstraints;
      }
    }

    public MappingAttributesAreSupportedForPropertyTypeValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var errorMessages = new StringBuilder();
      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        var validationResult = Validate (propertyDefinition.PropertyInfo);
        if (!validationResult.IsValid)
        {
          if (errorMessages.Length > 0)
            errorMessages.AppendLine (new string ('-', 10));
          errorMessages.AppendLine (validationResult.Message);
        }
      }

      var messages = errorMessages.ToString().Trim();
      return string.IsNullOrEmpty (messages) ? MappingValidationResult.CreateValidResult() : MappingValidationResult.CreateInvalidResult(messages);
    }

  //  //TODO 3424:
  //  // StringPropertyAttribute
  //  public class StringxxxValidationRule : xxxValidationRule<StringPropertyAttribute>
  //  {
  //    override string GetMEssage()
  //  }

  //  protected MappingValidationResultValidate (PropertyInfo pi)
  //{ 
  //    var attr = AttributeUtility.GetCustomAttributes<TAttribute> (pi, true))
  //    if (attr != null && !IsPropertyTypeSupported (pi))
  //    {
  //        return new MappingValidationResult (false, GetMessage (pi));
  //    }
  //    return new MappingValidationResult (true);
  //  }

    private MappingValidationResult Validate (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      foreach (Attribute attribute in AttributeUtility.GetCustomAttributes<Attribute> (propertyInfo, true))
      {
        var constraint = GetAttributeConstraint (attribute.GetType ());
        if (constraint != null && !Array.Exists (constraint.PropertyTypes, t => IsPropertyTypeSupported (propertyInfo, t)))
          return MappingValidationResult.CreateInvalidResult(constraint.Message);
      }
      return MappingValidationResult.CreateValidResult();
    }

    private bool IsPropertyTypeSupported (PropertyInfo propertyInfo, Type type)
    {
      if (type == typeof (ObjectList<>))
        return ReflectionUtility.IsObjectList (propertyInfo.PropertyType);
      return type.IsAssignableFrom (propertyInfo.PropertyType);
    }

    private AttributeConstraint GetAttributeConstraint (Type attributeType)
    {
      return (AttributeConstraints.Where (c => c.Key.IsAssignableFrom (attributeType)).Select (c => c.Value)).FirstOrDefault();
    }

    private void AddAttributeConstraints (Dictionary<Type, AttributeConstraint> attributeConstraints)
    {
      ArgumentUtility.CheckNotNull ("attributeConstraints", attributeConstraints);

      attributeConstraints.Add (typeof (StringPropertyAttribute), CreateAttributeConstraintForValueTypeProperty<StringPropertyAttribute, string> ());
      attributeConstraints.Add (typeof (BinaryPropertyAttribute), CreateAttributeConstraintForValueTypeProperty<BinaryPropertyAttribute, byte[]> ());
      attributeConstraints.Add (typeof (ExtensibleEnumPropertyAttribute), CreateAttributeConstraintForValueTypeProperty<ExtensibleEnumPropertyAttribute, IExtensibleEnum> ());
      attributeConstraints.Add (typeof (MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute> ());
      attributeConstraints.Add (typeof (DBBidirectionalRelationAttribute), CreateAttributeConstraintForRelationProperty<DBBidirectionalRelationAttribute>());
    }

    private AttributeConstraint CreateAttributeConstraintForValueTypeProperty<TAttribute, TProperty> ()
        where TAttribute : Attribute
    {
      return new AttributeConstraint (
          string.Format ("The '{0}' may be only applied to properties of type '{1}'.", typeof (TAttribute).Name, typeof (TProperty).Name),
          typeof (TProperty));
    }

    private AttributeConstraint CreateAttributeConstraintForRelationProperty<TAttribute> ()
        where TAttribute : Attribute
    {
      return new AttributeConstraint (
          string.Format (
              "The '{0}' may be only applied to properties assignable to types '{1}' or '{2}'.",
              typeof (TAttribute).Name,
              typeof (DomainObject).Name,
              typeof (ObjectList<>).Name),
          typeof (DomainObject),
          typeof (ObjectList<>));
    }
    
  }
}
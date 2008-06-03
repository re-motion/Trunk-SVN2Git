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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  internal class InterceptedPropertyCollector
  {
    private const BindingFlags _declaredInfrastructureBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private readonly Type _baseType;
    private readonly Set<Tuple<PropertyInfo, string>> _properties = new Set<Tuple<PropertyInfo, string>> ();
    private readonly Set<MethodInfo> _validatedMethods = new Set<MethodInfo> ();
    private readonly ClassDefinition _classDefinition;

    public InterceptedPropertyCollector (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      _baseType = baseType;

      _classDefinition = MappingConfiguration.Current.ClassDefinitions[_baseType];
      ValidateClassDefinition ();
      Assertion.IsTrue (_classDefinition is ReflectionBasedClassDefinition);

      AnalyzeAndValidateBaseType ();
    }

    public Set<Tuple<PropertyInfo, string>> GetProperties()
    {
      return _properties;
    }

    private void ValidateClassDefinition ()
    {
      if (_classDefinition == null)
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is not part of the mapping.", _baseType.FullName),
            _baseType);
      else if (_classDefinition.IsAbstract)
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is abstract and not instantiable.", _baseType.FullName),
         _baseType);
    }

    private void AnalyzeAndValidateBaseType ()
    {
      ValidateBaseType ();

      foreach (ReflectionBasedPropertyDefinition propertyDefinition in _classDefinition.GetPropertyDefinitions ())
      {
        PropertyInfo property = propertyDefinition.PropertyInfo;
        string propertyIdentifier = propertyDefinition.PropertyName;
        AnalyzeAndValidateProperty (property, propertyIdentifier);
      }

      foreach (IRelationEndPointDefinition endPointDefinition in _classDefinition.GetRelationEndPointDefinitions ())
      {
        if (endPointDefinition.IsVirtual)
        {
          Assertion.IsTrue (endPointDefinition is ReflectionBasedVirtualRelationEndPointDefinition);

          string propertyIdentifier = endPointDefinition.PropertyName;
          PropertyInfo property = ((ReflectionBasedVirtualRelationEndPointDefinition) endPointDefinition).PropertyInfo;

          AnalyzeAndValidateProperty (property, propertyIdentifier);
        }
      }

      ValidateRemainingMethods (_baseType);
    }

    private void AnalyzeAndValidateProperty (PropertyInfo property, string propertyIdentifier)
    {
      if (!property.DeclaringType.IsAssignableFrom (_baseType)) // we cannot intercept properties added from another class (mixin)
        return;
        
      MethodInfo getMethod = property.GetGetMethod (true);
      MethodInfo setMethod = property.GetSetMethod (true);

      ValidatePropertySetter (setMethod, property, propertyIdentifier);

      if (getMethod != null)
        _validatedMethods.Add (getMethod.GetBaseDefinition());
      if (setMethod != null)
        _validatedMethods.Add (setMethod.GetBaseDefinition ());

      _properties.Add (new Tuple<PropertyInfo, string> (property, propertyIdentifier));
    }

    private void ValidateBaseType ()
    {
      if (_baseType.IsSealed)
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is sealed.", _baseType.FullName), _baseType);
    }

    private void ValidateRemainingMethods (Type currentType)
    {
      foreach (MethodInfo method in currentType.GetMethods (_declaredInfrastructureBindingFlags))
      {
        if (method.IsAbstract && !_validatedMethods.Contains (method.GetBaseDefinition()))
          throw new NonInterceptableTypeException (
              string.Format (
                  "Cannot instantiate type {0} as its member {1} (on type {2}) is abstract (and not an automatic property).",
                  _baseType.FullName,
                  method.Name,
                  currentType.Name),
              _baseType);

        _validatedMethods.Add (method.GetBaseDefinition());
      }

      if (currentType.BaseType != null)
        ValidateRemainingMethods (currentType.BaseType);
    }

   private void ValidatePropertySetter (MethodInfo propertySetter, PropertyInfo property, string propertyIdentifier)
    {
      if (propertySetter != null && PropertyAccessor.GetPropertyKind (_classDefinition, propertyIdentifier) == PropertyKind.RelatedObjectCollection)
      {
        throw new NonInterceptableTypeException (
            string.Format (
                "Cannot instantiate type {0}, automatic properties for related object collections cannot have setters: property '{1}', property id '{2}'.",
                _baseType.FullName,
                property.Name,
                propertyIdentifier),
            _baseType);
      }
    }
  }
}

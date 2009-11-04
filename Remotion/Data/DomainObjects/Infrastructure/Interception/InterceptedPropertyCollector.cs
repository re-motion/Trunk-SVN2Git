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
using System.Runtime.CompilerServices;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  internal class InterceptedPropertyCollector
  {
    public static bool IsAutomaticPropertyAccessor (MethodInfo accessorMethod)
    {
      return accessorMethod != null && (accessorMethod.IsAbstract || accessorMethod.IsDefined (typeof (CompilerGeneratedAttribute), false));
    }

    public static bool IsOverridable (MethodInfo method)
    {
      return method != null && method.IsVirtual && !method.IsFinal;
    }

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

      if (getMethod != null)
      {
        ValidateAccessor (property, getMethod, "get accessor");
        _validatedMethods.Add (getMethod.GetBaseDefinition());
      }
      if (setMethod != null)
      {
        ValidateAccessor (property, setMethod, "set accessor");
        _validatedMethods.Add (setMethod.GetBaseDefinition());
      }

      _properties.Add (new Tuple<PropertyInfo, string> (property, propertyIdentifier));
    }

    private void ValidateAccessor (PropertyInfo property, MethodInfo accessor, string accessorDescription)
    {
      if (IsAutomaticPropertyAccessor (accessor) && !IsOverridable (accessor))
      {
        string message = string.Format ("Cannot instantiate type '{0}' as its member '{1}' has a non-virtual {2}.",
            _baseType.FullName, property.Name, accessorDescription);
        throw new NonInterceptableTypeException (message, _baseType);
      }
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
  }
}

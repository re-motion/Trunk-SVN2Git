// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Takes a mixin type and closes it when it is a generic type definitions, inferring its generic arguments from the mixin's target type and the
  /// parameters' constraints.
  /// </summary>
  public class MixinTypeCloser
  {
    private readonly ConstraintBasedGenericParameterInstantiator _parameterInstantiator = new ConstraintBasedGenericParameterInstantiator ();
    private readonly Type _targetClass;

    public MixinTypeCloser(Type targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      _targetClass = targetClass;
    }

    public Type GetClosedMixinType (Type configuredMixinType)
    {
      ArgumentUtility.CheckNotNull ("configuredMixinType", configuredMixinType);

      if (configuredMixinType.ContainsGenericParameters)
      {
        Type[] genericParameterInstantiations = GetGenericParameterInstantiations (configuredMixinType);
        try
        {
          return configuredMixinType.MakeGenericType (genericParameterInstantiations);
        }
        catch (ArgumentException ex)
        {
          var message = string.Format (
              "Cannot close the generic mixin type '{0}' applied to class '{1}' - the inferred type arguments violate the generic parameter "
              + "constraints. Specify the arguments manually, modify the parameter binding specification, or relax the constraints. {2}",
              configuredMixinType,
              _targetClass,
              ex.Message);
          throw new ConfigurationException (message, ex);
        }
      }
      else
        return configuredMixinType;
    }

    private Type[] GetGenericParameterInstantiations (Type mixinType)
    {
      var genericArguments = mixinType.GetGenericArguments ();
      var instantiations = new Type[genericArguments.Length];

      for (int i = 0; i < genericArguments.Length; i++)
        instantiations[i] = GetInstantiation (mixinType, genericArguments[i]);

      return instantiations;
    }

    private Type GetInstantiation (Type mixinType, Type genericArgument)
    {
      Assertion.IsTrue (genericArgument.IsGenericParameter, "Types with partially specified generic parameters are not supported.");

      Type instantiation = null;
      if (genericArgument.IsDefined (typeof (BindToTargetTypeAttribute), false))
        instantiation = _targetClass;
        
      if (genericArgument.IsDefined (typeof (BindToConstraintsAttribute), false))
      {
        if (instantiation != null)
        {
          var message = string.Format (
              "Type parameter '{0}' of mixin '{1}' has more than one binding specification.",
              genericArgument.Name,
              mixinType);
          throw new ConfigurationException (message);
        }

        instantiation = GetConstraintBasedInstantiation (mixinType, genericArgument);
      }

      if (instantiation == null)
      {
        string message = string.Format (
            "The generic mixin '{0}' applied to class '{1}' cannot be automatically closed because its type parameter '{2}' does not have any "
            + "binding information. Apply the BindToTargetTypeAttribute or the BindToConstraintsAttribute to the type parameter or specify the "
            + "parameter's instantiation when configuring the mixin for the target class.",
            mixinType,
            _targetClass,
            genericArgument.Name);
        throw new ConfigurationException (message);
      }

      return instantiation;
    }

    private Type GetConstraintBasedInstantiation (Type mixinType, Type genericArgument)
    {
      try
      {
        return _parameterInstantiator.Instantiate (genericArgument);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format (
            "The generic mixin '{0}' applied to class '{1}' cannot be automatically closed because the constraints of its type parameter '{2}' "
            + "cannot be unified into a single type. {3}",
            mixinType,
            _targetClass,
            genericArgument.Name,
            ex.Message);
        throw new ConfigurationException (message, ex);
      }
    }
  }
}

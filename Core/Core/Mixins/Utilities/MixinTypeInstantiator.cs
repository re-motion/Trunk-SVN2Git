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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  [Serializable]
  public class MixinTypeInstantiator
  {
    private readonly Type _targetClass;

    public MixinTypeInstantiator(Type targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      _targetClass = targetClass;
    }

    public Type GetClosedMixinType (Type configuredMixinType)
    {
      if (configuredMixinType.ContainsGenericParameters)
      {
        EnsureWellDefinedIntroductions (configuredMixinType);
        Type[] genericParameterInstantiations = GetGenericParameterInstantiations (configuredMixinType);
        return configuredMixinType.MakeGenericType (genericParameterInstantiations);
      }
      else
        return configuredMixinType;
    }

    private Type[] GetGenericParameterInstantiations (Type type)
    {
      List<Type> instantiations = new List<Type> ();
      foreach (Type genericArgument in type.GetGenericArguments ())
      {
        if (genericArgument.IsGenericParameter)
        {
          Type suggestion = _targetClass;
          if (ReflectionUtility.IsGenericParameterAssociatedWithAttribute (genericArgument, typeof (BaseAttribute)))
            suggestion = null; // cannot use the target class for a parameter bound to base argument

          Type instantiation;
          try
          {
            instantiation = GenericTypeInstantiator.GetGenericParameterInstantiation (genericArgument, suggestion);
          }
          catch (NotSupportedException ex)
          {
            string message = string.Format ("Invalid mixin configuration: {0}", ex.Message);
            throw new ConfigurationException (message, ex);
          }
          instantiations.Add (instantiation);
        }
      }
      return instantiations.ToArray ();
    }

    private List<Type> GetInterfacesBoundToThisParameter (Type configuredType)
    {
      List<Type> interfaces = new List<Type> ();
      foreach (Type t in configuredType.GetInterfaces ())
      {
        if (t.ContainsGenericParameters)
        {
          Type[] genericArguments = t.GetGenericArguments ();
          Type boundParameter = Array.Find (genericArguments, delegate (Type arg)
          {
            return arg.IsGenericParameter && ReflectionUtility.IsGenericParameterAssociatedWithAttribute (arg, typeof (ThisAttribute));
          });

          if (boundParameter != null)
            interfaces.Add (t);
        }
      }
      return interfaces;
    }

    private void EnsureWellDefinedIntroductions (Type configuredType)
    {
      Assertion.IsTrue (configuredType.ContainsGenericParameters);

      List<Type> introducedInterfaces = GetInterfacesBoundToThisParameter (configuredType);
      if (introducedInterfaces.Count > 0)
      {
        foreach (Type thisArgument in ReflectionUtility.GetGenericParametersAssociatedWithAttribute (configuredType, typeof (ThisAttribute)))
        {
          foreach (Type constraint in thisArgument.GetGenericParameterConstraints ())
          {
            if (!constraint.IsAssignableFrom (_targetClass))
            {
              string message = string.Format ("The generic interface {0} introduced by mixin {1} is bound to the mixin's This parameter. "
                + "That's is not allowed because the This parameter's value cannot be predicted as a result of the constraints being used. "
                + "Remove the constraints or bind the generic interface to another type or type parameter.", introducedInterfaces[0].Name,
                configuredType.FullName);
              throw new ConfigurationException (message);
            }
          }
        }
      }
    }
  }
}

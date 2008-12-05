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

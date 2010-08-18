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
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Implementation;
using Remotion.Reflection;

namespace Remotion.ServiceLocation
{
  public class DefaultServiceLocator : IServiceLocator
  {
    protected readonly InterlockedCache<Type, Func<object>> Cache = new InterlockedCache<Type, Func<object>>();

    public DefaultServiceLocator ()
    {
    }

    object IServiceProvider.GetService (Type serviceType)
    {
      return GetInstanceOrNull (serviceType);
    }

    public object GetInstance (Type serviceType)
    {
      var instance = GetInstanceOrNull (serviceType);
      if (instance == null)
      {
        string message = string.Format (
            "Cannot get a version-dependent implementation of type '{0}': " +
            "Expected 'ConcreteImplementationAttribute' could not be found.",
            serviceType.FullName);
        throw new ActivationException (message);
      }

      return instance;
    }

    public object GetInstance (Type serviceType, string key)
    {
      return GetInstance (serviceType);
    }

    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      var instance = GetInstanceOrNull (serviceType);
      if (instance != null)
        return new[] { instance };
      else
        return new object[0];
    }

    public TService GetInstance<TService> ()
    {
      return (TService) GetInstance (typeof (TService));
    }

    public TService GetInstance<TService> (string key)
    {
      return (TService) GetInstance (typeof (TService));
    }

    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      var instance = GetInstanceOrNull (typeof (TService));
      if (instance != null)
        return new[] { (TService) instance };
      else
        return new TService[0];
    }

    private object GetInstanceOrNull (Type serviceType)
    {
      return Cache.GetOrCreateValue (serviceType, GetInstanceFactory) ();
    }

    private Func<object> GetInstanceFactory (Type serviceType)
    {
      Func<object> factory;

      var concreteImplementationAttribute = GetConreteImplementationAttribute (serviceType);
      if (concreteImplementationAttribute == null)
        factory = () => null;
      else
      {
        var typeToInstantiate = concreteImplementationAttribute.ResolveType();
        var publicCtors = typeToInstantiate.GetConstructors().Where (ci => ci.IsPublic).ToArray();
        if (publicCtors.Length != 1)
          throw new InvalidOperationException (
              string.Format ("Type '{0}' has not exact one public constructor and cannot be instantiated.", typeToInstantiate.Name));

        var ctorInfo = publicCtors[0];
        var ctorParameters = ctorInfo.GetParameters();

        if (concreteImplementationAttribute.LifeTime == LifetimeKind.Instance)
        {
          if (ctorParameters.Length == 0)
            factory = (Func<object>) new ConstructorLookupInfo (typeToInstantiate).GetDelegate (typeof (Func<object>));
          else
            factory = () =>
            {
              var ctorArgs = ctorParameters.Select (constructorParameter => GetInstance (constructorParameter.ParameterType)).ToArray();

              // TODO: The goal is to have a delegate that calls: new MyType ((int) ctorArgs[0], (string) ctorArgs[1])
              return ctorInfo.Invoke (ctorArgs);
            };
        }
        else
        {
          var ctorArgs = ctorParameters.Select (constructorParameter => GetInstance (constructorParameter.ParameterType)).ToArray ();
          var instance = Activator.CreateInstance (typeToInstantiate, ctorArgs);
          factory = () => instance;
        }
      }
      return factory;
    }

    private ConcreteImplementationAttribute GetConreteImplementationAttribute (Type serviceType)
    {
      var attributes = serviceType.GetCustomAttributes (typeof (ConcreteImplementationAttribute), false);
      if (attributes.Length == 0)
        return null;
      return (ConcreteImplementationAttribute) attributes[0];
    }
  }
}
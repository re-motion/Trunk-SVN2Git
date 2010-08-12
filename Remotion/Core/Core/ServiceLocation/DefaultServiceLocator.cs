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
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Implementation;
using Remotion.Reflection;

namespace Remotion.ServiceLocation
{
  public class DefaultServiceLocator : IServiceLocator
  {
    public static readonly DefaultServiceLocator Instance = new DefaultServiceLocator();
    protected readonly InterlockedCache<Type, Func<object>> Cache = new InterlockedCache<Type, Func<object>>();

    protected DefaultServiceLocator ()
    {
    }

    object IServiceProvider.GetService (Type serviceType)
    {
      try
      {
        return GetInstance (serviceType);
      }
      catch (Exception)
      {
        return null;
      }
    }

    public object GetInstance (Type serviceType)
    {
      Func<object> instanceCreator;
      if (Cache.TryGetValue (serviceType, out instanceCreator))
        return ((Func<object>) instanceCreator())();

      var concreteImplementationAttribute = GetConreteImplementationAttribute (serviceType);
      var instance = concreteImplementationAttribute.InstantiateType();

      instanceCreator = () => Activator.CreateInstance (instance.GetType());
      Cache.GetOrCreateValue (serviceType, t => () => instanceCreator);
      return instance;
    }

    public object GetInstance (Type serviceType, string key)
    {
      return GetInstance (serviceType);
    }

    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      return new[] { GetInstance (serviceType) };
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
      return new[] { (TService) GetInstance (typeof (TService)) };
    }

    private ConcreteImplementationAttribute GetConreteImplementationAttribute (Type serviceType)
    {
      var attributes = serviceType.GetCustomAttributes (typeof (ConcreteImplementationAttribute), false);
      if (attributes.Length == 0)
        throw new ActivationException ("The requested service does not have the ConcreteImplementationAttribute applied.");
      return (ConcreteImplementationAttribute) attributes[0];
    }
  }
}
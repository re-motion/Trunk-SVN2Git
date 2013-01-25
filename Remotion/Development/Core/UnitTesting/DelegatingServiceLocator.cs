// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// A <see cref="IServiceLocator"/> that allows runtime registration of service implementations. If no mapping is found, the service locator
  /// delegats to the inner <see cref="IServiceLocator"/> instance.
  /// </summary>
  public class DelegatingServiceLocator : ServiceLocatorImplBase
  {
    private readonly Dictionary<Type, Func<object>> _getInstancesCreators = new Dictionary<Type, Func<object>>();
    private readonly Dictionary<Type, Func<IEnumerable<object>>> _getAllInstancesCreators = new Dictionary<Type, Func<IEnumerable<object>>>();

    private readonly IServiceLocator _serviceLocator;

    public DelegatingServiceLocator (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      _serviceLocator = serviceLocator;
    }

    public void Register (Type serviceType, Func<object> creator)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("creator", creator);

      _getInstancesCreators.Add (serviceType, creator);
    }

    public void RegisterAll (Type serviceType, Func<IEnumerable<object>> creator)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("creator", creator);

      _getAllInstancesCreators.Add (serviceType, creator);
    }

    protected override object DoGetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      // key is ignored.

      var creator = _getInstancesCreators.GetValueOrDefault (serviceType);
      if (creator != null)
        return creator();
      else
        return _serviceLocator.GetInstance (serviceType, key);
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      var creator = _getAllInstancesCreators.GetValueOrDefault (serviceType);
      if (creator != null)
        return creator();
      else
        return _serviceLocator.GetAllInstances (serviceType);
    }
  }
}
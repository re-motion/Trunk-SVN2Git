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
using Remotion.BridgeInterfaces;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Context;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();
    private readonly IServiceLocator _defaultServiceLocator = new DefaultServiceLocator();

    public StubServiceLocator ()
    {
      _instances.Add (typeof (IThemedResourceUrlResolverFactory), new StubResourceUrlResolverFactory());
      _instances.Add (typeof (IScriptUtility), MockRepository.GenerateStub<IScriptUtility>());
      _instances.Add (typeof (ILogManager), new Log4NetLogManager());
      _instances.Add (typeof (IAdapterRegistryImplementation), MockRepository.GenerateStub<IAdapterRegistryImplementation> ());
    }

    public void SetFactory<T> (T factory)
    {
      _instances[typeof (T)] = factory;
    }

    protected override object DoGetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      if (IsCoreType (serviceType))
        return _defaultServiceLocator.GetInstance (serviceType, key);

      return _instances.GetOrCreateValue (
          serviceType, delegate (Type type) { throw new ArgumentException (string.Format ("No service for type '{0}' registered.", type)); });
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      if (IsCoreType (serviceType))
        return _defaultServiceLocator.GetAllInstances (serviceType);

      object serviceInstance;
      if (_instances.TryGetValue (serviceType, out serviceInstance))
        return new[] { serviceInstance };
      return new object[0];
    }

    private bool IsCoreType (Type serviceType)
    {
      return serviceType.Assembly != typeof (HttpContextStorageProvider).Assembly;
    }
  }
}
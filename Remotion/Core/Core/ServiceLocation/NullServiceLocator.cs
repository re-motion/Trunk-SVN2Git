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
using Microsoft.Practices.ServiceLocation;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Null-object implementation of <see cref="IServiceLocator"/>. Use the <see cref="Instance"/> member if you do not have a concrete 
  /// <see cref="IServiceLocator"/> implementation to set to <see cref="ServiceLocator.Current"/>.
  /// </summary>
  public sealed class NullServiceLocator : IServiceLocator
  {
    public static readonly NullServiceLocator Instance = new NullServiceLocator();

    private NullServiceLocator ()
    {
    }

    object IServiceProvider.GetService (Type serviceType)
    {
      return null;
    }

    public object GetInstance (Type serviceType)
    {
      throw CreateActivationException();
    }

    public object GetInstance (Type serviceType, string key)
    {
      throw CreateActivationException();
    }

    public TService GetInstance<TService> ()
    {
      throw CreateActivationException();
    }

    public TService GetInstance<TService> (string key)
    {
      throw CreateActivationException();
    }

    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      return new object[0];
    }

    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      return new TService[0];
    }

    private ActivationException CreateActivationException ()
    {
      return new ActivationException (
          "The NullServiceLocator cannot be used to retrieve a single instance for a type. "
          + "Please configure your application to use a concrete implementation of IServiceLocator.");
    }
  }
}
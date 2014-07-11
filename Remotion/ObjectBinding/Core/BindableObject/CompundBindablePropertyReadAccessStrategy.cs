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
using System.Linq;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Combines one or more <see cref="IBindablePropertyReadAccessStrategy"/>-instances and delegates checking if the property can be read from.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IBindablePropertyReadAccessStrategy), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompundBindablePropertyReadAccessStrategy : IBindablePropertyReadAccessStrategy
  {
    private readonly IReadOnlyList<IBindablePropertyReadAccessStrategy> _bindablePropertyReadAccessStrategies;

    public CompundBindablePropertyReadAccessStrategy (IEnumerable<IBindablePropertyReadAccessStrategy> bindablePropertyReadAccessStrategies)
    {
      ArgumentUtility.CheckNotNull ("bindablePropertyReadAccessStrategies", bindablePropertyReadAccessStrategies);

      _bindablePropertyReadAccessStrategies = bindablePropertyReadAccessStrategies.ToList().AsReadOnly();
    }

    public IReadOnlyList<IBindablePropertyReadAccessStrategy> BindablePropertyReadAccessStrategies
    {
      get { return _bindablePropertyReadAccessStrategies; }
    }


    public bool CanRead (BindableObjectClass bindableClass, PropertyBase bindableProperty, IBusinessObject businessObject)
    {
      ArgumentUtility.DebugCheckNotNull ("bindableClass", bindableClass);
      ArgumentUtility.DebugCheckNotNull ("bindableProperty", bindableProperty);
      // businessObject can be null

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return _strategies.All (s => s.CanRead (propertyBase, businessObject));
      // ReSharper disable LoopCanBeConvertedToQuery
      // ReSharper disable ForCanBeConvertedToForeach
      for (int i = 0; i < _bindablePropertyReadAccessStrategies.Count; i++)
      {
        if (!_bindablePropertyReadAccessStrategies[i].CanRead (bindableClass, bindableProperty, businessObject))
          return false;
      }
      return true;
      // ReSharper restore ForCanBeConvertedToForeach
      // ReSharper restore LoopCanBeConvertedToQuery
    }
  }
}
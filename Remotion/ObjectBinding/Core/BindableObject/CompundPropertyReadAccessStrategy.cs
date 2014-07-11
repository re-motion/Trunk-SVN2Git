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
  /// Combines one or more <see cref="IPropertyReadAccessStrategy"/>-instances and delegates checking if the property can be read from.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IPropertyReadAccessStrategy), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompundPropertyReadAccessStrategy : IPropertyReadAccessStrategy
  {
    private readonly IReadOnlyList<IPropertyReadAccessStrategy> _propertyReadAccessStrategies;

    public CompundPropertyReadAccessStrategy (IEnumerable<IPropertyReadAccessStrategy> propertyReadAccessStrategies)
    {
      ArgumentUtility.CheckNotNull ("propertyReadAccessStrategies", propertyReadAccessStrategies);

      _propertyReadAccessStrategies = propertyReadAccessStrategies.ToList().AsReadOnly();
    }

    public IReadOnlyList<IPropertyReadAccessStrategy> PropertyReadAccessStrategies
    {
      get { return _propertyReadAccessStrategies; }
    }


    public bool CanRead (PropertyBase propertyBase, IBusinessObject businessObject)
    {
      ArgumentUtility.DebugCheckNotNull ("propertyBase", propertyBase);
      ArgumentUtility.DebugCheckNotNull ("businessObject", businessObject);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return _strategies.All (s => s.CanRead (propertyBase, businessObject));
      // ReSharper disable LoopCanBeConvertedToQuery
      // ReSharper disable ForCanBeConvertedToForeach
      //for (int i = 0; i < _strategies.Count; i++)
      //{
      //  if (!_strategies[i].CanRead (propertyBase, businessObject))
      //    return false;
      //}
      return true;
      // ReSharper restore ForCanBeConvertedToForeach
      // ReSharper restore LoopCanBeConvertedToQuery
    }
  }
}
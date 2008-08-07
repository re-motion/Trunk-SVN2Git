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
using System.Linq;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Used for creating the outer product of a <see cref="SecurableClassDefinition"/>'s <see cref="SecurableClassDefinition.StateProperties"/>'
  /// <see cref="StateDefinition"/> values.
  /// </summary>
  public class StateCombinationBuilder : IStateCombinationBuilder
  {
    private readonly SecurableClassDefinition _classDefinition;

    public StateCombinationBuilder (SecurableClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _classDefinition = classDefinition;
    }

    public SecurableClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public PropertyStateTuple[][] CreatePropertyProduct ()
    {
      IEnumerable<IEnumerable<PropertyStateTuple>> seed = new PropertyStateTuple[][] { };
      //_classDefinition.StateProperties.Select (p => p.DefinedStates.DefaultIfEmpty().Select (state => new PropertyStateTuple (p, state)));
      
      var allStatesByProperty = from property in _classDefinition.StateProperties
                                select from state in property.DefinedStates.DefaultIfEmpty()
                                       select new PropertyStateTuple (property, state);

      var aggregatedStates = allStatesByProperty.Aggregate (seed, CreateOuterProduct);

      return aggregatedStates.Select (innerList => innerList.ToArray()).ToArray();
    }

    private IEnumerable<IEnumerable<PropertyStateTuple>> CreateOuterProduct (
        IEnumerable<IEnumerable<PropertyStateTuple>> previous,
        IEnumerable<PropertyStateTuple> current)
    {
      return from p in previous.DefaultIfEmpty (new PropertyStateTuple[0])
             from c in current
             select p.Concat (new[] { c });
    }
  }
}
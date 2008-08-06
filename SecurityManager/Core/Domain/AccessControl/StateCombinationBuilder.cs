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
  public class StateCombinationBuilder
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

    public StateDefinition[][] CreatePropertyProduct ()
    {
      IEnumerable<IEnumerable<StateDefinition>> seed = new StateDefinition[][] { };
      var allStatesByProperty = _classDefinition.StateProperties.Select (property => property.DefinedStates);
      var aggregatedStates = allStatesByProperty.Aggregate (seed, (previous, current) => CreateOuterProduct (previous, current));

      return aggregatedStates.Select (innerList => innerList.ToArray ()).ToArray ();
    }

    private IEnumerable<IEnumerable<StateDefinition>> CreateOuterProduct (
        IEnumerable<IEnumerable<StateDefinition>> previous,
        IEnumerable<StateDefinition> current)
    {
      return from p in previous.DefaultIfEmpty (new StateDefinition[0])
             from c in current.DefaultIfEmpty ()
             select p.Concat (new[] { c });
    }
  }
}

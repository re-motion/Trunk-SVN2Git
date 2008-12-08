// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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

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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a set of <see cref="StateType"/> values, allowing efficient <see cref="StateType"/> matching via the <see cref="Matches"/> method.
  /// </summary>
  public struct StateValueSet
  {
    private readonly bool _matchDiscarded;
    private readonly bool _matchDeleted;
    private readonly bool _matchChanged;
    private readonly bool _matchUnchanged;
    private readonly bool _matchNew;
#warning TODO 2054: private readonly bool _notLoadedYet;

    public StateValueSet (params StateType[] stateValues) : this()
    {
      foreach (var stateValue in stateValues)
      {
        switch (stateValue)
        {
          case StateType.Discarded:
            _matchDiscarded = true;
            break;
          case StateType.Deleted:
            _matchDeleted = true;
            break;
          case StateType.Changed:
            _matchChanged = true;
            break;
          case StateType.Unchanged:
            _matchUnchanged = true;
            break;
          case StateType.New:
            _matchNew = true;
            break;
          default:
            throw new ArgumentOutOfRangeException ("stateValues", stateValue, "Invalid StateType value.");
        }
      }
    }

    public bool Matches (StateType stateValue)
    {
      switch (stateValue)
      {
        case StateType.Discarded:
          return _matchDiscarded;
        case StateType.Deleted:
          return _matchDeleted;
        case StateType.Changed:
          return _matchChanged;
        case StateType.Unchanged:
          return _matchUnchanged;
        case StateType.New:
          return _matchNew;
        default:
          throw new ArgumentOutOfRangeException ("stateValue", stateValue, "Invalid StateType value.");
      }
    }
  }
}
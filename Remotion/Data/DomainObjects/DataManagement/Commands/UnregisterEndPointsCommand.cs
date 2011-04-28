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
using System.Collections.ObjectModel;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Unregisters a set of end-points from a <see cref="RelationEndPointMap"/>.
  /// </summary>
  public class UnregisterEndPointsCommand : IDataManagementCommand
  {
    private readonly RelationEndPointID[] _endPointIDs;
    private readonly RelationEndPointMap _map;

    public UnregisterEndPointsCommand (IEnumerable<RelationEndPointID> endPointIDs, RelationEndPointMap map)
    {
      ArgumentUtility.CheckNotNull ("endPointIDs", endPointIDs);
      ArgumentUtility.CheckNotNull ("map", map);

      _endPointIDs = endPointIDs.ToArray();
      _map = map;
    }

    public ReadOnlyCollection<RelationEndPointID> EndPointIDs
    {
      get { return Array.AsReadOnly (_endPointIDs); }
    }

    public RelationEndPointMap Map
    {
      get { return _map; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return new Exception[0];
    }

    public void NotifyClientTransactionOfBegin ()
    {
      // Nothing to do
    }

    public void Begin ()
    {
      // Nothing to do
    }

    public void Perform ()
    {
      foreach (var endPointID in _endPointIDs)
      {
        if (!endPointID.Definition.IsVirtual)
        {
          _map.UnregisterRealObjectEndPoint (endPointID);
        }
        else
        {
          _map.RemoveEndPoint (endPointID);
        }
      }
    }

    public void End ()
    {
      // Nothing to do
    }

    public void NotifyClientTransactionOfEnd ()
    {
      // Nothing to do
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}

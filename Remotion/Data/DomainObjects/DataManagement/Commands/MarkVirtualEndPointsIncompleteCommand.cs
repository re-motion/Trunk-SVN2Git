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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Marks a virtual end-point incomplete.
  /// </summary>
  public class MarkVirtualEndPointsIncompleteCommand : IDataManagementCommand
  {
    private readonly IVirtualEndPoint[] _virtualEndPoints;

    public MarkVirtualEndPointsIncompleteCommand (IEnumerable<IVirtualEndPoint> virtualEndPoints)
    {
      ArgumentUtility.CheckNotNull ("virtualEndPoints", virtualEndPoints);
      
      _virtualEndPoints = virtualEndPoints.ToArray();
    }

    public IVirtualEndPoint[] VirtualEndPoints
    {
      get { return _virtualEndPoints; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception>();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      // Nothing to do here
    }

    public void Begin ()
    {
      // Nothing to do here
    }

    public void Perform ()
    {
      foreach (var virtualEndPoint in _virtualEndPoints)
      {
        if (virtualEndPoint.IsDataComplete)
          virtualEndPoint.MarkDataIncomplete ();
      }
    }

    public void End ()
    {
      // Nothing to do here
    }

    public void NotifyClientTransactionOfEnd ()
    {
      // Nothing to do here
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
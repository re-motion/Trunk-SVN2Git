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

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implements the <see cref="IVirtualEndPointStateUpdateListener"/> interface by passing information about state updates on to a 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class VirtualEndPointStateUpdateListener : IVirtualEndPointStateUpdateListener
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;

    public VirtualEndPointStateUpdateListener (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      _clientTransaction = clientTransaction;
      _endPointID = endPointID;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public void StateUpdated (bool? newChangedState)
    {
      _clientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, newChangedState);
    }
  }
}
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
using System.Linq;
using Remotion.Utilities;
using ArgumentUtility=Remotion.Data.Linq.Utilities.ArgumentUtility;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Encapsulates all logic that is required to delete a <see cref="DomainObject"/>.
  /// </summary>
  public class DeleteCommand : IDataManagementCommand
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly DomainObject _deletedObject;
    private readonly DataContainer _dataContainer;
    private readonly CompositeCommand _endPointDeleteCommands;

    public DeleteCommand (ClientTransaction clientTransaction, DomainObject deletedObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);

      _clientTransaction = clientTransaction;
      _deletedObject = deletedObject;

      _dataContainer = _clientTransaction.GetDataContainer (_deletedObject);
      Assertion.IsFalse (_dataContainer.State == StateType.Deleted);
      Assertion.IsFalse (_dataContainer.State == StateType.Discarded);
      
      _endPointDeleteCommands = new CompositeCommand (
          from endPointID in _dataContainer.AssociatedRelationEndPointIDs
          let endPoint = _clientTransaction.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID)
          select endPoint.CreateDeleteCommand());
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public DomainObject DeletedObject
    {
      get { return _deletedObject; }
    }

    public void NotifyClientTransactionOfBegin ()
    {
      _clientTransaction.Execute (delegate
      {
        _clientTransaction.TransactionEventSink.ObjectDeleting (_deletedObject);
        _endPointDeleteCommands.NotifyClientTransactionOfBegin ();
      });
    }

    public void Begin ()
    {
      _clientTransaction.Execute (delegate
      {
        _deletedObject.OnDeleting (EventArgs.Empty);
        _endPointDeleteCommands.Begin ();
      });
    }

    public void Perform ()
    {
      _endPointDeleteCommands.Perform ();

      if (_dataContainer.State == StateType.New)
        _clientTransaction.DataManager.Discard (_dataContainer);
      else
        _dataContainer.Delete();
    }

    public void End ()
    {
      _clientTransaction.Execute (delegate
      {
        _endPointDeleteCommands.End ();
        _deletedObject.OnDeleted (EventArgs.Empty);
      });
    }

    public void NotifyClientTransactionOfEnd ()
    {
      _clientTransaction.Execute (delegate
      {
        _endPointDeleteCommands.NotifyClientTransactionOfEnd ();
        _clientTransaction.TransactionEventSink.ObjectDeleted (_deletedObject);
      });
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var allOppositeRelationEndPoints = 
          OppositeRelationEndPointFinder.GetOppositeRelationEndPoints (_clientTransaction.DataManager.RelationEndPointMap, _deletedObject);

      var commands = from oppositeEndPoint in allOppositeRelationEndPoints
                     select oppositeEndPoint.CreateRemoveCommand (_deletedObject);

      return new ExpandedCommand (this).CombineWith (commands);
    }
  }
}
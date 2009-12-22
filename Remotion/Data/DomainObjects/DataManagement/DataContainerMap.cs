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
using System.Collections;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class DataContainerMap : IEnumerable, IFlattenedSerializable
{
  // types

  // static members and constants

  // member fields

  private readonly ClientTransaction _clientTransaction;
  private readonly IClientTransactionListener _transactionEventSink;
  private readonly DataContainerCollection _dataContainers;

  // construction and disposing

  public DataContainerMap (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
    _transactionEventSink = clientTransaction.TransactionEventSink;
    _dataContainers = new DataContainerCollection ();
  }

  // methods and properties

  public DataContainer this[ObjectID id]
  {
    get { return _dataContainers[id]; }
  }

  public int Count
  {
    get { return _dataContainers.Count; }
  }

  public IEnumerable<DataContainer> GetByState (StateType state)
  {
    return _dataContainers.Cast<DataContainer> ().Where (dc => dc.State == state);
  }

  public void CommitAllDataContainers ()
  {
    foreach (DataContainer dataContainer in _dataContainers)
      dataContainer.CommitState ();
  }

  public void RollbackAllDataContainers ()
  {
    foreach (DataContainer dataContainer in _dataContainers)
      dataContainer.RollbackState();
  }

  public DomainObject GetObjectWithoutLoading (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    if (_dataContainers.Contains (id))
    {
      DataContainer dataContainer = this[id];

      if (!includeDeleted && dataContainer.State == StateType.Deleted)
        throw new ObjectDeletedException (id);

      return dataContainer.DomainObject;
    }
    else
      return null;
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    _transactionEventSink.DataContainerMapRegistering (dataContainer);
    _dataContainers.Add (dataContainer);
  }

  public void PerformDelete (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    CheckClientTransactionForDeletion (dataContainer);

    if (dataContainer.State == StateType.New)
      Remove (dataContainer.ID);    
  }

  public void Remove (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    
    var dataContainer = this[id];
    if (dataContainer == null)
    {
      var message = string.Format ("Data container '{0}' is not part of this map.", id);
      throw new ArgumentException (message, "id");
    }

    _transactionEventSink.DataContainerMapUnregistering (dataContainer);
    _dataContainers.Remove (dataContainer);
  }

  public DataContainerCollection MergeWithRegisteredDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.Merge (_dataContainers);
  }

  public DataContainerCollection GetNotRegisteredDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.GetDifference (_dataContainers);
  }

  private void CheckClientTransactionForDeletion (DataContainer dataContainer)
  {
    if (dataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot remove DataContainer '{0}' from DataContainerMap, because it belongs to a different ClientTransaction.",
          dataContainer.ID);
    }
  }

  private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
  {
    return new ClientTransactionsDifferException (string.Format (message, args));
  }

  #region IEnumerable Members

  public IEnumerator GetEnumerator ()
  {
    return _dataContainers.GetEnumerator ();
  }

  #endregion

  #region Serialization
  protected DataContainerMap (FlattenedDeserializationInfo info)
      : this (info.GetValueForHandle<ClientTransaction>())
  {
    var dataContainerCount = info.GetValue<int> ();
    for (int i = 0; i < dataContainerCount; ++i)
      _dataContainers.Add (info.GetValueForHandle<DataContainer> ());
  }

  void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
  {
    info.AddHandle (_clientTransaction);
    
    info.AddValue (_dataContainers.Count);
    foreach (DataContainer dataContainer in _dataContainers)
      info.AddHandle (dataContainer);
  }
  #endregion
}
}

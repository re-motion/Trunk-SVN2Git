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
    ArgumentUtility.CheckValidEnumValue ("state", state);

    return _dataContainers.Cast<DataContainer> ().Where (dc => dc.State == state);
  }

  public void Commit ()
  {
    foreach (DataContainer dataContainer in _dataContainers)
      dataContainer.Commit ();
  }

  public void Rollback ()
  {
    foreach (DataContainer dataContainer in _dataContainers)
      dataContainer.Rollback();
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

  public void PerformDelete2 (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    CheckClientTransactionForDeletion (dataContainer);

    if (dataContainer.State == StateType.New)
      Discard2(dataContainer);    
  }

  private void Discard2 (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    _transactionEventSink.DataContainerMapUnregistering (dataContainer);

    _dataContainers.Remove (dataContainer);
  }

  public void Discard (ObjectID id)
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
    dataContainer.Discard ();
  }

  public void Commit2 ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];
      
      if (dataContainer.State == StateType.Deleted)
        Discard2 (dataContainer);

      dataContainer.Commit2 ();
    }
  }

  public void Rollback2 ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];

      Rollback2(dataContainer);
    }
  }

  public void Rollback2 (DataContainer dataContainer)
  {
    if (dataContainer.State == StateType.New)
      Discard2 (dataContainer);

    dataContainer.Rollback2 ();
  }

  public DomainObjectCollection GetByState2 (StateType state)
  {
    ArgumentUtility.CheckValidEnumValue ("state", state);

    DomainObjectCollection domainObjects = new DomainObjectCollection ();

    DataContainerCollection dataContainers = _dataContainers.GetByState (state);
    foreach (DataContainer dataContainer in dataContainers)
      domainObjects.Add (dataContainer.DomainObject);

    return domainObjects;
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
    DataContainer[] dataContainers = info.GetArray<DataContainer>();
    foreach (DataContainer dataContainer in dataContainers)
      _dataContainers.Add (dataContainer);
  }

  void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
  {
    info.AddHandle (_clientTransaction);
    DataContainer[] dataContainers = new DataContainer[_dataContainers.Count];
    _dataContainers.CopyTo (dataContainers, 0);
    info.AddArray (dataContainers);
  }
  #endregion
}
}

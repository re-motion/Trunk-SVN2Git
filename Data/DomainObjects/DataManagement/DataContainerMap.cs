// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

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

  public DomainObject GetObjectWithoutLoading (ObjectID id, bool includeDeleted)
  {
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
      Remove(dataContainer);    
  }

  private void Remove (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    _transactionEventSink.DataContainerMapUnregistering (dataContainer);

    _dataContainers.Remove (dataContainer);
  }

  public void Commit ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];
      
      if (dataContainer.State == StateType.Deleted)
        Remove (dataContainer);

      dataContainer.Commit ();
    }
  }

  public void Rollback ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];

      Rollback(dataContainer);
    }
  }

  public void Rollback (DataContainer dataContainer)
  {
    if (dataContainer.State == StateType.New)
      Remove (dataContainer);

    dataContainer.Rollback ();
  }

  public DomainObjectCollection GetByState (StateType state)
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

  public void CopyFrom (DataContainerMap source)
  {
    ArgumentUtility.CheckNotNull ("source", source);

    if (source == this)
      throw new ArgumentException ("Source cannot be the destination DataContainerMap instance.");

    _transactionEventSink.DataContainerMapCopyingFrom (source);
    source._transactionEventSink.DataContainerMapCopyingTo (this);

    int startingPosition = _dataContainers.Count;

    for (int i = 0; i < source._dataContainers.Count; ++i)
    {
      DataContainer newContainer = source._dataContainers[i].Clone ();
      newContainer.SetClientTransaction (_clientTransaction);
      int position = _dataContainers.Add (newContainer);
      Assertion.IsTrue (position == i + startingPosition);
    }
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

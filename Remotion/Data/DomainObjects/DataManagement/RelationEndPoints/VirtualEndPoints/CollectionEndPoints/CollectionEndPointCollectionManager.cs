// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements <see cref="ICollectionEndPointCollectionManager"/> by storing the active <see cref="DomainObjectCollection"/> instances for a
  /// <see cref="ClientTransaction"/> in a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  [Serializable]
  public class CollectionEndPointCollectionManager : ICollectionEndPointCollectionManager
  {
    private readonly SimpleDataStore<RelationEndPointID, DomainObjectCollection> _originalCollectionReferences =
        new SimpleDataStore<RelationEndPointID, DomainObjectCollection>();
    private readonly SimpleDataStore<RelationEndPointID, DomainObjectCollection> _currentCollectionReferences =
        new SimpleDataStore<RelationEndPointID, DomainObjectCollection> ();
    private readonly IAssociatedCollectionDataStrategyFactory _dataStrategyFactory;

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _transactionEventSink;

    public CollectionEndPointCollectionManager (
        IAssociatedCollectionDataStrategyFactory dataStrategyFactory, 
        ClientTransaction clientTransaction, 
        IClientTransactionListener transactionEventSink)
    {
      ArgumentUtility.CheckNotNull ("dataStrategyFactory", dataStrategyFactory);
      _dataStrategyFactory = dataStrategyFactory;
      _clientTransaction = clientTransaction;
      _transactionEventSink = transactionEventSink;
    }

    public IAssociatedCollectionDataStrategyFactory DataStrategyFactory
    {
      get { return _dataStrategyFactory; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionListener TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public DomainObjectCollection GetOriginalCollectionReference (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return _originalCollectionReferences.GetOrCreateValue (
          endPoint.ID,
          id => CreateCollection (id, _dataStrategyFactory.CreateDataStrategyForEndPoint (endPoint)));
    }

    public DomainObjectCollection GetCurrentCollectionReference (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return _currentCollectionReferences.GetOrCreateValue (endPoint.ID, id => GetOriginalCollectionReference (endPoint));
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint endPoint, IDomainObjectCollectionData originalData)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return CreateCollection (endPoint.ID, originalData);
    }

    public void AssociateCollectionWithEndPoint (ICollectionEndPoint endPoint, DomainObjectCollection newCollection)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);

      var oldCollection = (IAssociatableDomainObjectCollection) GetCurrentCollectionReference (endPoint);
      Assertion.IsTrue (oldCollection.IsAssociatedWith (endPoint));
      oldCollection.TransformToStandAlone();

      ((IAssociatableDomainObjectCollection) newCollection).TransformToAssociated (endPoint, _dataStrategyFactory);
      _currentCollectionReferences[endPoint.ID] = newCollection;
      _transactionEventSink.VirtualRelationEndPointStateUpdated (_clientTransaction, endPoint.ID, null);
    }

    public bool HasCollectionReferenceChanged (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var originalCollection = _originalCollectionReferences.GetValueOrDefault (endPoint.ID);
      if (originalCollection == null)
      {
        Assertion.DebugAssert (!_currentCollectionReferences.ContainsKey (endPoint.ID));
        return false;
      }

      var currentCollection = _currentCollectionReferences.GetValueOrDefault (endPoint.ID);
      if (currentCollection == null)
        return false;

      return currentCollection != originalCollection;
    }

    public void CommitCollectionReference (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var originalCollection = _originalCollectionReferences.GetValueOrDefault (endPoint.ID);
      if (originalCollection == null)
      {
        Assertion.DebugAssert (!_currentCollectionReferences.ContainsKey (endPoint.ID));
        return;
      }

      var currentCollection = _currentCollectionReferences.GetValueOrDefault (endPoint.ID);
      if (currentCollection == null)
        return;

      _originalCollectionReferences[endPoint.ID] = currentCollection;
      Assertion.DebugAssert (!HasCollectionReferenceChanged (endPoint));
    }

    public void RollbackCollectionReference (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      try
      {
        var originalCollection = _originalCollectionReferences.GetValueOrDefault (endPoint.ID);
        if (originalCollection == null)
        {
          Assertion.DebugAssert (!_currentCollectionReferences.ContainsKey (endPoint.ID));
          return;
        }

        var currentCollection = _currentCollectionReferences.GetValueOrDefault (endPoint.ID);
        if (currentCollection == null)
          return;

        if (originalCollection == currentCollection)
          return;

        // If the end-point's current collection is still associated with this end point, transform it to stand-alone.
        // (During rollback, the current relation might have already been associated with another end-point, we must not overwrite this!)
        var oldCollection = (IAssociatableDomainObjectCollection) GetCurrentCollectionReference (endPoint);
        if (oldCollection.IsAssociatedWith (endPoint))
          oldCollection.TransformToStandAlone();

        // We must always associate the new collection with the end point, however - even during rollback phase,
        ((IAssociatableDomainObjectCollection) originalCollection).TransformToAssociated (endPoint, _dataStrategyFactory);
        _currentCollectionReferences[endPoint.ID] = originalCollection;

        // TODO 4527: Consider whether this method should raise the VirtualRelationEndPointStateUpdated. If so, Commit should probably raise it as well.
        // TODO 4527: Also note that ChangeCachingCollectionDataDecorator raises StateUpdated (false) events that are translated into
        // VirtualRelationEndPointStateUpdated (false) events without taking the reference change state into account.
        _transactionEventSink.VirtualRelationEndPointStateUpdated (_clientTransaction, endPoint.ID, null);
      }
      finally
      {
        Assertion.DebugAssert (!HasCollectionReferenceChanged (endPoint));
      }
    }

    private DomainObjectCollection CreateCollection (RelationEndPointID endPointID, IDomainObjectCollectionData dataStrategy)
    {
      return DomainObjectCollectionFactory.Instance.CreateCollection (endPointID.Definition.PropertyInfo.PropertyType, dataStrategy);
    }
  }
}
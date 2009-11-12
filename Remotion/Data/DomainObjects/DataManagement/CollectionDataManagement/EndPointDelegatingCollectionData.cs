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
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/> by forwarding all requests to an implementation of 
  /// <see cref="ICollectionEndPoint"/>.
  /// </summary>
  public class EndPointDelegatingCollectionData : IDomainObjectCollectionData
  {
    private readonly ICollectionEndPoint _collectionEndPoint;
    private readonly IDomainObjectCollectionData _actualData; // TODO 1766: Should be IReadOnlyDomainObjectCollectionData

    public EndPointDelegatingCollectionData (ICollectionEndPoint collectionEndPoint, IDomainObjectCollectionData actualData)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("actualData", actualData);

      _collectionEndPoint = collectionEndPoint;
      _actualData = actualData;
    }

    public ICollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public int Count
    {
      get { return _actualData.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _actualData.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.IndexOf (objectID);
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _actualData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    // TODO 1780: Inline calls to ICollectionChangeDelegate - move code from RelationEndPointModifier here

    public void Clear ()
    {
      for (int i = Count - 1; i >= 0; --i)
        (CollectionEndPoint).PerformRemove (null, GetObject (i));
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot insert DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      CheckNotDeleted (domainObject);
      CheckNotDeleted (CollectionEndPoint.GetDomainObject());

      var insertModification = CollectionEndPoint.CreateInsertModification (domainObject, index);
      var bidirectionalModification = insertModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();

      CollectionEndPoint.Touch ();
    }

    public void Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      CheckNotDeleted (domainObject);
      CheckNotDeleted (CollectionEndPoint.GetDomainObject());

      if (ContainsObjectID (domainObject.ID))
      {
        var modification = CollectionEndPoint.CreateRemoveModification (domainObject);
        var bidirectionalModification = modification.CreateBidirectionalModification ();
        bidirectionalModification.ExecuteAllSteps ();
      }

      CollectionEndPoint.Touch ();
    }

    public void Replace (int index, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      (CollectionEndPoint).PerformReplace (null, newDomainObject, index);
    }

    private void CheckClientTransaction (DomainObject domainObject, string exceptionFormatString)
    {
      if (!domainObject.TransactionContext[CollectionEndPoint.ClientTransaction].CanBeUsedInTransaction)
      {
        string transactionInfo = GetTransactionInfoForMismatchingClientTransactions (domainObject);

        var formattedMessage = string.Format (
            exceptionFormatString, 
            domainObject.ID, 
            CollectionEndPoint.Definition.PropertyName, 
            CollectionEndPoint.ObjectID);
        throw new ClientTransactionsDifferException (formattedMessage + " The objects do not belong to the same ClientTransaction." + transactionInfo);
      }
    }

    private string GetTransactionInfoForMismatchingClientTransactions (DomainObject otherDomainObject)
    {
      var transactionInfo = new StringBuilder ();

      var endPointObject = CollectionEndPoint.GetDomainObject ();
      if (otherDomainObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (" The {0} object is bound to a BindingClientTransaction.", otherDomainObject.GetPublicDomainObjectType ().Name);
        if (endPointObject.HasBindingTransaction)
        {
          transactionInfo.AppendFormat (
              " The {0} object owning the collection is also bound, but to a different BindingClientTransaction.",
              endPointObject.GetPublicDomainObjectType ().Name);
        }
      }
      else if (endPointObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (
            " The {0} object owning the collection is bound to a BindingClientTransaction.", 
            endPointObject.GetPublicDomainObjectType ().Name);
      }
      return transactionInfo.ToString();
    }

    private void CheckNotDeleted (DomainObject domainObject)
    {
      if (domainObject.TransactionContext[CollectionEndPoint.ClientTransaction].State == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }
  }
}

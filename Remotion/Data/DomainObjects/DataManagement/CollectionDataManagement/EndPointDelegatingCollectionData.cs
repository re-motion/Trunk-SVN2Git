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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/> by forwarding all requests to an implementation of 
  /// <see cref="ICollectionEndPoint"/>.
  /// </summary>
  [Serializable]
  public class EndPointDelegatingCollectionData : IDomainObjectCollectionData
  {
    [NonSerialized] // relies on the collection end point restoring the association on deserialization
    private readonly ICollectionEndPoint _associatedEndPoint; 
    private readonly IDomainObjectCollectionData _actualData;

    public EndPointDelegatingCollectionData (ICollectionEndPoint collectionEndPoint, IDomainObjectCollectionData actualData)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("actualData", actualData);

      _associatedEndPoint = collectionEndPoint;
      _actualData = actualData;
    }

    public int Count
    {
      get { return _actualData.Count; }
    }

    public Type RequiredItemType
    {
      get { return _actualData.RequiredItemType; }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _associatedEndPoint; }
    }

    public IDomainObjectCollectionData GetUndecoratedDataStore ()
    {
      return _actualData.GetUndecoratedDataStore();
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

    public void Clear ()
    {
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, AssociatedEndPoint.GetDomainObject ());

      for (int i = Count - 1; i >= 0; --i)
      {
        var removedObject = GetObject (i);

        Assertion.IsTrue (removedObject.TransactionContext[AssociatedEndPoint.ClientTransaction].State != StateType.Deleted);
        CreateAndExecuteRemoveModification (removedObject);
      }

      AssociatedEndPoint.Touch ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, domainObject, "Cannot insert DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, domainObject);
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, AssociatedEndPoint.GetDomainObject ());

      var insertModification = AssociatedEndPoint.CreateInsertModification (domainObject, index);
      var bidirectionalModification = insertModification.CreateRelationModification ();
      bidirectionalModification.ExecuteAllSteps ();

      AssociatedEndPoint.Touch ();
    }

    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, domainObject);
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, AssociatedEndPoint.GetDomainObject ());

      var containsObjectID = ContainsObjectID (domainObject.ID);
      if (containsObjectID)
        CreateAndExecuteRemoveModification (domainObject);

      AssociatedEndPoint.Touch ();
      return containsObjectID;
    }

    public bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, AssociatedEndPoint.GetDomainObject ());

      var domainObject = GetObject (objectID);
      if (domainObject != null)
      {
        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (domainObject.TransactionContext[AssociatedEndPoint.ClientTransaction].State != StateType.Deleted);

        CreateAndExecuteRemoveModification (domainObject);
      }

      AssociatedEndPoint.Touch ();
      return domainObject != null;
    }

    public void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, value, "Cannot put DomainObject '{0}' into the collection of property '{1}' of DomainObject '{2}'.");
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, value);
      RelationEndPointValueChecker.CheckNotDeleted (AssociatedEndPoint, AssociatedEndPoint.GetDomainObject ());

      var replaceModification = AssociatedEndPoint.CreateReplaceModification (index, value);
      var bidirectionalModification = replaceModification.CreateRelationModification ();
      bidirectionalModification.ExecuteAllSteps ();

      AssociatedEndPoint.Touch ();
    }

    private void CreateAndExecuteRemoveModification (DomainObject domainObject)
    {
      var modification = AssociatedEndPoint.CreateRemoveModification (domainObject);
      var bidirectionalModification = modification.CreateRelationModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }
  }
}

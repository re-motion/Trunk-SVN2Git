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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints
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

    public EndPointDelegatingCollectionData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      _associatedEndPoint = collectionEndPoint;
    }

    public int Count
    {
      get
      {
        var data = _associatedEndPoint.GetData();
        return data.Count;
      }
    }

    public Type RequiredItemType
    {
      get 
      {
        // Currently, the data backing a CollectionEndPoint does not check the item type.
        // This is hard-coded (rather than delegating to _associatedEndPoint.GetData().RequiredItemType) to avoid lazy loading for
        // item type checks.
        return null; 
      }
    }

    public bool IsReadOnly
    {
      get 
      { 
        // Currently, the data backing a CollectionEndPoint is never read-only.
        // This is hard-coded (rather than delegating to _associatedEndPoint.GetData().IsReadOnly) because that always returns a read-only
        // decorator.
        return false; 
      }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _associatedEndPoint; }
    }

    public bool IsDataComplete
    {
      get { return _associatedEndPoint.IsDataComplete; }
    }

    public void EnsureDataComplete ()
    {
      _associatedEndPoint.EnsureDataComplete ();
    }

    public IDomainObjectCollectionData GetDataStore ()
    {
      // This will usually return the ChangeCachingDomainObjectCollectionData
      var data = _associatedEndPoint.GetData ();
      return data.GetDataStore();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = _associatedEndPoint.GetData ();
      return data.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      var data = _associatedEndPoint.GetData ();
      return data.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = _associatedEndPoint.GetData ();
      return data.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = _associatedEndPoint.GetData ();
      return data.IndexOf(objectID);
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      var data = _associatedEndPoint.GetData ();
      return data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Clear ()
    {
      DomainObjectCheckUtility.EnsureNotDeleted (AssociatedEndPoint.GetDomainObjectReference (), AssociatedEndPoint.ClientTransaction);

      var combinedCommand = GetClearCommand ();
      combinedCommand.NotifyAndPerform ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, domainObject, "Cannot insert DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (domainObject, AssociatedEndPoint.ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (AssociatedEndPoint.GetDomainObjectReference(), AssociatedEndPoint.ClientTransaction);

      var insertCommand = AssociatedEndPoint.CreateInsertCommand (domainObject, index);
      var bidirectionalModification = insertCommand.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();

      AssociatedEndPoint.Touch ();
    }

    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (domainObject, AssociatedEndPoint.ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (AssociatedEndPoint.GetDomainObjectReference (), AssociatedEndPoint.ClientTransaction);

      var containsObjectID = ContainsObjectID (domainObject.ID);
      if (containsObjectID)
        CreateAndExecuteRemoveCommand (domainObject);

      AssociatedEndPoint.Touch ();
      return containsObjectID;
    }

    public bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      DomainObjectCheckUtility.EnsureNotDeleted (AssociatedEndPoint.GetDomainObjectReference(), AssociatedEndPoint.ClientTransaction);

      var domainObject = GetObject (objectID);
      if (domainObject != null)
      {
        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (domainObject.TransactionContext[AssociatedEndPoint.ClientTransaction].State != StateType.Deleted);

        CreateAndExecuteRemoveCommand (domainObject);
      }

      AssociatedEndPoint.Touch ();
      return domainObject != null;
    }

    public void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      RelationEndPointValueChecker.CheckClientTransaction (AssociatedEndPoint, value, "Cannot put DomainObject '{0}' into the collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (value, AssociatedEndPoint.ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (AssociatedEndPoint.GetDomainObjectReference (), AssociatedEndPoint.ClientTransaction);

      var replaceCommand = AssociatedEndPoint.CreateReplaceCommand (index, value);
      var bidirectionalModification = replaceCommand.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();

      AssociatedEndPoint.Touch ();
    }

    private void CreateAndExecuteRemoveCommand (DomainObject domainObject)
    {
      var command = AssociatedEndPoint.CreateRemoveCommand (domainObject);
      var bidirectionalModification = command.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();
    }

    private CompositeCommand GetClearCommand ()
    {
      var removeCommands = new List<ExpandedCommand> ();

      for (int i = Count - 1; i >= 0; --i)
      {
        var removedObject = GetObject (i);

        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (removedObject.TransactionContext[AssociatedEndPoint.ClientTransaction].State != StateType.Deleted);
        removeCommands.Add (AssociatedEndPoint.CreateRemoveCommand (removedObject).ExpandToAllRelatedObjects ());
      }

      return new CompositeCommand (removeCommands.Cast<IDataManagementCommand> ()).CombineWith (new RelationEndPointTouchCommand (AssociatedEndPoint));
    }
  }
}

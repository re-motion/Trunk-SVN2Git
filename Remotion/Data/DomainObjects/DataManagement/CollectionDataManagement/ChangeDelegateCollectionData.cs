// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  //public class ChangeDelegateCollectionData
  //{
  //  private readonly DomainObjectCollectionData _data = new DomainObjectCollectionData ();

  //  private readonly ICollectionChangeDelegate _changeDelegate;
  //  private readonly IDomainObjectCollectionEventRaiser _eventRaiser;
  //  private DomainObjectCollection _parentCollection;

  //  public ChangeDelegateCollectionData (
  //      ICollectionChangeDelegate changeDelegate, 
  //      IDomainObjectCollectionEventRaiser eventRaiser, 
  //      DomainObjectCollection parentCollection)
  //  {
  //    ArgumentUtility.CheckNotNull ("changeDelegate", changeDelegate);
  //    ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
  //    ArgumentUtility.CheckNotNull ("parentCollection", parentCollection);

  //    _changeDelegate = changeDelegate;
  //    _eventRaiser = eventRaiser;
  //    _parentCollection = parentCollection;
  //  }

  //  public ICollectionChangeDelegate ChangeDelegate
  //  {
  //    get { return _changeDelegate; }
  //  }

  //  public override int Count
  //  {
  //    get { return _data.Count; }
  //  }

  //  public override bool IsReadOnly
  //  {
  //    get { return false; }
  //  }

  //  public override bool ContainsObjectID (ObjectID objectID)
  //  {
  //    ArgumentUtility.CheckNotNull ("objectID", objectID);
  //    return _data.ContainsObjectID (objectID);
  //  }

  //  public override DomainObject GetObject (int index)
  //  {
  //    return _data.GetObject (index);
  //  }

  //  public override DomainObject GetObject (ObjectID objectID)
  //  {
  //    ArgumentUtility.CheckNotNull ("objectID", objectID);
  //    return _data.GetObject (objectID);
  //  }

  //  public override int IndexOf (ObjectID objectID)
  //  {
  //    ArgumentUtility.CheckNotNull ("objectID", objectID);
  //    return _data.IndexOf (objectID);
  //  }

  //  public override IEnumerator<DomainObject> GetEnumerator ()
  //  {
  //    return _data.GetEnumerator ();
  //  }

  //  protected override void PerformClear ()
  //  {
  //    for (int i = Count - 1; i >= 0; --i)
  //      _changeDelegate.PerformRemove (_parentCollection, GetObject (i));
  //  }

  //  protected override void PerformInsert (int index, DomainObject domainObject)
  //  {
  //    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

  //    // TODO: change delegate
  //    throw new System.NotImplementedException ();
  //  }

  //  protected override void PerformRemove (int index, ObjectID objectID)
  //  {
  //    // TODO: change delegate
  //    throw new System.NotImplementedException ();
  //  }

  //  protected override void PerformReplace (int index, ObjectID oldDomainObjectID, DomainObject newDomainObject)
  //  {
  //    // TODO: change delegate
  //    throw new System.NotImplementedException ();
  //  }

  //  protected override void IncrementVersion ()
  //  {
  //    // nothing to do here
  //  }

  //  public void RaiseBeginAddEvent ()
  //  {
  //    // TODO: eventRaiser
  //    throw new System.NotImplementedException ();
  //  }

  //  public void RaiseEndAddEvent ()
  //  {
  //    // TODO: eventRaiser
  //    throw new System.NotImplementedException ();
  //  }

  //  public void RaiseBeginRemoveEvent ()
  //  {
  //    // TODO: eventRaiser
  //    throw new System.NotImplementedException ();
  //  }

  //  public void RaiseEndRemoveEvent ()
  //  {
  //    // TODO: eventRaiser
  //    throw new System.NotImplementedException ();
  //  }

  //  public void InsertData (int index, DomainObject domainObject)
  //  {
  //    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

  //    _data.Insert (index, domainObject);
  //  }

  //  public void RemoveData (ObjectID objectID)
  //  {
  //    ArgumentUtility.CheckNotNull ("objectID", objectID);

  //    _data.Remove (objectID);
  //  }

  //  public void ReplaceData (ObjectID oldDomainObjectID, DomainObject newDomainObject)
  //  {
  //    ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
  //    ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

  //    _data.Replace (oldDomainObjectID, newDomainObject);
  //  }
  //}
}
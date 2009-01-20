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
  /// <summary>
  /// Extends <see cref="DomainObjectCollectionData"/> by events being raised whenever the collection is modified. The events are raised via
  /// an <see cref="IDomainObjectCollectionEventRaiser"/> instance.
  /// </summary>
  /// <remarks>
  /// The reason this class derives from <see cref="DomainObjectCollectionData"/> rather than wrapping an <see cref="IDomainObjectCollectionData"/>
  /// instance is that it guarantees that the modification events are definitely only raised when the modification takes place. Argument checks
  /// and semantic checks are performed before the events are raised.
  /// </remarks>
  [Serializable]
  public class EventRaisingCollectionData : DomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionEventRaiser _eventRaiser;

    public EventRaisingCollectionData (IDomainObjectCollectionEventRaiser eventRaiser)
    {
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
      _eventRaiser = eventRaiser;
    }

    protected override void PerformClear ()
    {
      var removedObjects = new Stack<DomainObject> ();

      int index = 0;
      foreach (var domainObject in this)
      {
        _eventRaiser.BeginRemove (index, domainObject);
        removedObjects.Push (domainObject);
        ++index;
      }

      Assertion.IsTrue (index == Count);

      base.PerformClear ();

      foreach (var domainObject in removedObjects)
      {
        --index;
        _eventRaiser.EndRemove (index, domainObject);
      }

      Assertion.IsTrue (index == 0);
    }

    protected override void PerformInsert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _eventRaiser.BeginAdd (index, domainObject);
      base.PerformInsert (index, domainObject);
      _eventRaiser.EndAdd (index, domainObject);
    }

    protected override void PerformRemove (int index, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var domainObject = GetObject (index);

      _eventRaiser.BeginRemove (index, domainObject);
      base.PerformRemove (index, objectID);
      _eventRaiser.EndRemove (index, domainObject);
    }

    protected override void PerformReplace (int index, ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      var oldDomainObject = GetObject (oldDomainObjectID);
      
      _eventRaiser.BeginRemove (index, oldDomainObject);
      _eventRaiser.BeginAdd (index, newDomainObject);
      base.PerformReplace (index, oldDomainObjectID, newDomainObject);
      _eventRaiser.EndRemove (index, oldDomainObject);
      _eventRaiser.EndAdd (index, newDomainObject);
    }
  }
}
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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Decorates <see cref="IDomainObjectCollectionData"/> by raising events whenever the inner collection is modified. The events are raised via
  /// an <see cref="IDomainObjectCollectionEventRaiser"/> instance before and after the modification.
  /// </summary>
  [Serializable]
  public class EventRaisingCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    private readonly IDomainObjectCollectionEventRaiser _eventRaiser;

    public EventRaisingCollectionDataDecorator (IDomainObjectCollectionEventRaiser eventRaiser, IDomainObjectCollectionData wrappedData)
      : base (wrappedData)
    {
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
      _eventRaiser = eventRaiser;
    }

    public IDomainObjectCollectionEventRaiser EventRaiser
    {
      get { return _eventRaiser; }
    }

    public override void Clear ()
    {
      var removedObjects = new Stack<DomainObject> (); // holds the removed objects in order to raise 

      int index = 0;
      foreach (var domainObject in this)
      {
        _eventRaiser.BeginRemove (index, domainObject);
        removedObjects.Push (domainObject);
        ++index;
      }

      Assertion.IsTrue (index == Count);

      WrappedData.Clear ();

      foreach (var domainObject in removedObjects)
      {
        --index;
        _eventRaiser.EndRemove (index, domainObject);
      }

      Assertion.IsTrue (index == 0);
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _eventRaiser.BeginAdd (index, domainObject);
      WrappedData.Insert (index, domainObject);
      _eventRaiser.EndAdd (index, domainObject);
    }

    public override bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      
      int index = IndexOf (domainObject.ID);
      if (index == -1)
        return false;

      _eventRaiser.BeginRemove (index, domainObject);
      WrappedData.Remove (domainObject);
      _eventRaiser.EndRemove (index, domainObject);

      return true;
    }

    public override bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      int index = IndexOf (objectID);
      if (index == -1)
        return false;
      
      var domainObject = GetObject (objectID);
      _eventRaiser.BeginRemove (index, domainObject);
      WrappedData.Remove (objectID);
      _eventRaiser.EndRemove (index, domainObject);
      
      return true;
    }

    public override void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var oldDomainObject = GetObject (index);
      if (oldDomainObject != value)
      {
        _eventRaiser.BeginRemove (index, oldDomainObject);
        _eventRaiser.BeginAdd (index, value);
        WrappedData.Replace (index, value);
        _eventRaiser.EndRemove (index, oldDomainObject);
        _eventRaiser.EndAdd (index, value);
      }
    }
  }
}

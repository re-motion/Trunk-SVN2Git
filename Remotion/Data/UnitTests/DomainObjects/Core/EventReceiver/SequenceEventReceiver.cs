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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  [Serializable]
  public class SequenceEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private DomainObject[] _domainObjects;
    private DomainObjectCollection[] _collections;
    private ArrayList _states = new ArrayList ();
    private int _cancelEventNumber = 0;

    // construction and disposing

    public SequenceEventReceiver (DomainObjectCollection collection)
      : this (new DomainObject[0], new DomainObjectCollection[] { collection })
    {
    }

    public SequenceEventReceiver (DomainObject domainObject)
      : this (new DomainObject[] { domainObject }, new DomainObjectCollection[0])
    {
    }

    public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections)
      : this (domainObjects, collections, 0)
    {
    }

    public SequenceEventReceiver (DomainObjectCollection collection, int cancelEventNumber)
      : this (new DomainObject[0], new DomainObjectCollection[] { collection }, cancelEventNumber)
    {
    }

    public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections, int cancelEventNumber)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull ("collections", collections);

      _domainObjects = domainObjects;
      _collections = collections;
      _cancelEventNumber = cancelEventNumber;

      foreach (DomainObject domainObject in domainObjects)
      {
        domainObject.Deleting += new EventHandler (DomainObject_Deleting);
        domainObject.Deleted += new EventHandler (DomainObject_Deleted);
        domainObject.PropertyChanging += new PropertyChangeEventHandler (DomainObject_PropertyChanging);
        domainObject.PropertyChanged += new PropertyChangeEventHandler (DomainObject_PropertyChanged);
        domainObject.RelationChanging += new RelationChangingEventHandler (DomainObject_RelationChanging);
        domainObject.RelationChanged += new RelationChangedEventHandler (DomainObject_RelationChanged);
      }

      foreach (DomainObjectCollection collection in collections)
      {
        collection.Adding += new DomainObjectCollectionChangeEventHandler (Collection_Changing);
        collection.Added += new DomainObjectCollectionChangeEventHandler (Collection_Changed);
        collection.Removing += new DomainObjectCollectionChangeEventHandler (Collection_Changing);
        collection.Removed += new DomainObjectCollectionChangeEventHandler (Collection_Changed);
      }
    }

    // methods and properties

    public int CancelEventNumber
    {
      get { return _cancelEventNumber; }
      set { _cancelEventNumber = value; }
    }

    public ChangeState this[int index]
    {
      get { return (ChangeState) _states[index]; }
    }

    public int Count
    {
      get { return _states.Count; }
    }

    public void Check (ChangeState[] expectedStates)
    {
      for (int i = 0; i < expectedStates.Length; i++)
      {
        if (i >= _states.Count)
          Assert.Fail ("Missing event: " + expectedStates[i].Message);

        try
        {
          ((ChangeState) _states[i]).Check (expectedStates[i]);
        }
        catch (Exception e)
        {
          Assert.Fail (string.Format ("{0}: {1}", expectedStates[i].Message, e.Message));
        }
      }

      Assert.AreEqual (expectedStates.Length, _states.Count, "Length");
    }

    public void Unregister ()
    {
      foreach (DomainObject domainObject in _domainObjects)
      {
        domainObject.Deleting -= new EventHandler (DomainObject_Deleting);
        domainObject.Deleted -= new EventHandler (DomainObject_Deleted);
        domainObject.PropertyChanging -= new PropertyChangeEventHandler (DomainObject_PropertyChanging);
        domainObject.PropertyChanged -= new PropertyChangeEventHandler (DomainObject_PropertyChanged);
        domainObject.RelationChanging -= new RelationChangingEventHandler (DomainObject_RelationChanging);
        domainObject.RelationChanged -= new RelationChangedEventHandler (DomainObject_RelationChanged);
      }

      foreach (DomainObjectCollection collection in _collections)
      {
        collection.Adding -= new DomainObjectCollectionChangeEventHandler (Collection_Changing);
        collection.Added -= new DomainObjectCollectionChangeEventHandler (Collection_Changed);
        collection.Removing -= new DomainObjectCollectionChangeEventHandler (Collection_Changing);
        collection.Removed -= new DomainObjectCollectionChangeEventHandler (Collection_Changed);
      }
    }

    public void DomainObject_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _states.Add (new PropertyChangeState (sender, args.PropertyValue, args.OldValue, args.NewValue));
    }

    public void DomainObject_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _states.Add (new PropertyChangeState (sender, args.PropertyValue, args.OldValue, args.NewValue));

      if (_states.Count == _cancelEventNumber)
        CancelOperation ();
    }

    public void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      _states.Add (new RelationChangeState (sender, args.PropertyName, args.OldRelatedObject, args.NewRelatedObject));

      if (_states.Count == _cancelEventNumber)
        CancelOperation ();
    }

    public void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      _states.Add (new RelationChangeState (sender, args.PropertyName, null, null));
    }

    public void DomainObject_Deleting (object sender, EventArgs args)
    {
      _states.Add (new ObjectDeletionState (sender));

      if (_states.Count == _cancelEventNumber)
        CancelOperation ();
    }

    public void DomainObject_Deleted (object sender, EventArgs args)
    {
      _states.Add (new ObjectDeletionState (sender));
    }

    public void Collection_Changing (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _states.Add (new CollectionChangeState (sender, args.DomainObject));

      if (_states.Count == _cancelEventNumber)
        CancelOperation ();
    }

    public void Collection_Changed (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _states.Add (new CollectionChangeState (sender, args.DomainObject));
    }
  }
}

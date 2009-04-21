// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  [Serializable]
  public class DomainObjectCollectionEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private bool _cancel;

    private DomainObject _addingDomainObject;
    private DomainObject _addedDomainObject;
    private bool _hasAddingEventBeenCalled;
    private bool _hasAddedEventBeenCalled;

    private DomainObjectCollection _removingDomainObjects;
    private DomainObjectCollection _removedDomainObjects;
    private bool _hasRemovingEventBeenCalled;
    private bool _hasRemovedEventBeenCalled;

    // construction and disposing

    public DomainObjectCollectionEventReceiver (DomainObjectCollection collection)
      : this (collection, false)
    {
    }

    public DomainObjectCollectionEventReceiver (DomainObjectCollection collection, bool cancel)
    {
      _cancel = cancel;

      collection.Adding += new DomainObjectCollectionChangeEventHandler (DomainObjectCollection_Adding);
      collection.Added += new DomainObjectCollectionChangeEventHandler (DomainObjectCollection_Added);

      _removingDomainObjects = new DomainObjectCollection ();
      _removedDomainObjects = new DomainObjectCollection ();
      collection.Removing += new DomainObjectCollectionChangeEventHandler (DomainObjectCollection_Removing);
      collection.Removed += new DomainObjectCollectionChangeEventHandler (DomainObjectCollection_Removed);
    }

    // methods and properties

    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    public DomainObject AddingDomainObject
    {
      get { return _addingDomainObject; }
    }

    public DomainObject AddedDomainObject
    {
      get { return _addedDomainObject; }
    }

    public bool HasAddingEventBeenCalled
    {
      get { return _hasAddingEventBeenCalled; }
    }

    public bool HasAddedEventBeenCalled
    {
      get { return _hasAddedEventBeenCalled; }
    }

    public DomainObjectCollection RemovingDomainObjects
    {
      get { return _removingDomainObjects; }
    }

    public DomainObjectCollection RemovedDomainObjects
    {
      get { return _removedDomainObjects; }
    }

    public bool HasRemovingEventBeenCalled
    {
      get { return _hasRemovingEventBeenCalled; }
    }

    public bool HasRemovedEventBeenCalled
    {
      get { return _hasRemovedEventBeenCalled; }
    }

    private void DomainObjectCollection_Adding (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _hasAddingEventBeenCalled = true;
      _addingDomainObject = args.DomainObject;

      if (_cancel)
        CancelOperation ();
    }

    private void DomainObjectCollection_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _addedDomainObject = args.DomainObject;
      _hasAddedEventBeenCalled = true;
    }

    private void DomainObjectCollection_Removing (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _hasRemovingEventBeenCalled = true;
      _removingDomainObjects.Add (args.DomainObject);

      if (_cancel)
        CancelOperation ();
    }

    private void DomainObjectCollection_Removed (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _removedDomainObjects.Add (args.DomainObject);
      _hasRemovedEventBeenCalled = true;
    }
  }
}

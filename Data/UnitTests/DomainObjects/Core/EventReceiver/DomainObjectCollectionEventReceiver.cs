/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

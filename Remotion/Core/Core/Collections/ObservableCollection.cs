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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Extends <see cref="Collection{T}"/> with events that indicate when the collection was changed.
  /// </summary>
  /// <typeparam name="T">The type of items held by this <see cref="ObservableCollection{T}"/>.</typeparam>
  public class ObservableCollection<T> : Collection<T>
  {
    private class ChangeResistantEnumerable : IEnumerable<T>
    {
      private readonly ObservableCollection<T> _collection;

      public ChangeResistantEnumerable (ObservableCollection<T> collection)
      {
        ArgumentUtility.CheckNotNull ("collection", collection);
        _collection = collection;
      }

      public IEnumerator<T> GetEnumerator ()
      {
        return new ChangeResistantObservableCollectionEnumerator<T> (_collection);
      }

      IEnumerator IEnumerable.GetEnumerator ()
      {
        return GetEnumerator ();
      }
    }

    /// <summary>
    /// Occurs after the items of this <see cref="ObservableCollection{T}"/> have been cleared.
    /// </summary>
    public event EventHandler ItemsCleared;
    /// <summary>
    /// Occurs after an item has been removed from this <see cref="ObservableCollection{T}"/>. It does not occur when an item is replaced, in this
    /// case the <see cref="ItemSet"/> event is raised.
    /// </summary>
    public event EventHandler<ObservableCollectionChangedEventArgs<T>> ItemRemoved;
    /// <summary>
    /// Occurs after an item has been added to this <see cref="ObservableCollection{T}"/>. It does not occur when an item is replaced, in this
    /// case the <see cref="ItemSet"/> event is raised.
    /// </summary>
    public event EventHandler<ObservableCollectionChangedEventArgs<T>> ItemInserted;
    /// <summary>
    /// Occurs after an item has been set at a specific index of this <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public event EventHandler<ObservableCollectionChangedEventArgs<T>> ItemSet;

    protected override void ClearItems ()
    {
      base.ClearItems ();

      if (ItemsCleared != null)
        ItemsCleared (this, EventArgs.Empty);
    }
    
    protected override void RemoveItem (int index)
    {
      var item = this[index];

      base.RemoveItem (index);

      if (ItemRemoved != null)
        ItemRemoved (this, new ObservableCollectionChangedEventArgs<T> (index, item));
    }

    protected override void InsertItem (int index, T item)
    {
      base.InsertItem (index, item);

      if (ItemInserted != null)
        ItemInserted (this, new ObservableCollectionChangedEventArgs<T> (index, item));
    }

    protected override void SetItem (int index, T item)
    {
      base.SetItem (index, item);

      if (ItemSet != null)
        ItemSet (this, new ObservableCollectionChangedEventArgs<T> (index, item));
    }

    /// <summary>
    /// Returns an instance of <see cref="IEnumerable{T}"/> that represents this collection but can be enumerated even while the collection changes
    /// (see <see cref="ChangeResistantObservableCollectionEnumerator{T}"/>).
    /// </summary>
    public IEnumerable<T> AsChangeResistantEnumerable ()
    {
      return new ChangeResistantEnumerable (this);
    }
  }
}
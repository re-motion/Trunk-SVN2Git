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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Decorates another <see cref="IDomainObjectCollectionData"/> object and provides an <see cref="HasChanged"/> method that determines
  /// whether the contents of that <see cref="IDomainObjectCollectionData"/> object has changed when compared to an original 
  /// <see cref="DomainObjectCollection"/> instance. The result of the comparison is cached until the <see cref="OnDataChanged"/> method is
  /// called or a modifying method is called on <see cref="ChangeCachingCollectionDataDecorator"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class also manages the <see cref="OriginalData"/> associated with the changed data. The original data collection is a 
  /// <see cref="CopyOnWriteDomainObjectCollectionData"/> and is exposed only through a read-only wrapper. As a result, the 
  /// <see cref="ChangeCachingCollectionDataDecorator"/> class is the only class that can change that original data.
  /// </para>
  /// </remarks>
  [Serializable]
  public class ChangeCachingCollectionDataDecorator : ObservableCollectionDataDecorator
  {
    private readonly CopyOnWriteDomainObjectCollectionData _originalData;
    private readonly ICollectionDataStateUpdateListener _stateUpdateListener;

    private bool _isCacheUpToDate;
    private bool _cachedHasChangedFlag;

    public ChangeCachingCollectionDataDecorator (
        IDomainObjectCollectionData wrappedData, 
        ICollectionDataStateUpdateListener stateUpdateListener)
      : base (ArgumentUtility.CheckNotNull ("wrappedData", wrappedData))
    {
      ArgumentUtility.CheckNotNull ("stateUpdateListener", stateUpdateListener);

      _originalData = new CopyOnWriteDomainObjectCollectionData (this);
      _stateUpdateListener = stateUpdateListener;
    }

    public IDomainObjectCollectionData OriginalData
    {
      get { return new ReadOnlyCollectionDataDecorator (_originalData, false); }
    }

    public bool IsCacheUpToDate
    {
      get { return _isCacheUpToDate; }
    }

    public bool HasChanged (ICollectionEndPointChangeDetectionStrategy strategy)
    {
      if (!_isCacheUpToDate)
      {
        var hasChanged = strategy.HasDataChanged (this, OriginalData);
        SetCachedHasChangedFlag (hasChanged);
      }

      return _cachedHasChangedFlag;
    }

    public override IDomainObjectCollectionData GetDataStore ()
    {
      return this;
    }

    public void Commit ()
    {
      _originalData.RevertToCopiedData();
      SetCachedHasChangedFlag (false);
    }

    /// <summary>
    /// Registers the given <paramref name="domainObject"/> as an original item of this collection. This means the item is added to the 
    /// <see cref="OriginalData"/> collection, and it is also added to this <see cref="ChangeCachingCollectionDataDecorator"/> collection. If the 
    /// <see cref="OriginalData"/> collection already contains the item, an exception is thrown. If this collection already contains the item, it is
    /// only added to the <see cref="OriginalData"/>. This operation may invalidate the state cache.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to be registered.</param>
    public void RegisterOriginalItem (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (_originalData.ContainsObjectID (domainObject.ID))
      {
        var message = string.Format ("The original collection already contains a domain object with ID '{0}'.", domainObject.ID);
        throw new InvalidOperationException (message);
      }

      if (!WrappedData.ContainsObjectID (domainObject.ID))
      {
        // Standard case: Neither collection contains the item; the item is added to both, and the state cache stays valid

        // Add the item to the WrappedData collection. That way, if the original collection has not yet been copied, it will automatically contain the 
        // item and the state cache remains valid.
        WrappedData.Add (domainObject);

        // If the original collection has been copied, we must add the item manually. The state cache still remains valid because we always add
        // the item at the end. If the collections were equal before, they remain equal now. If they were different before, they remain different.
        if (_originalData.IsContentsCopied)
          _originalData.Add (domainObject);
      }
      else
      {
        // Special case: The current collection already contains the item

        // We must add the item to the original collection only and raise a potential state change notification
        _originalData.Add (domainObject);
        OnChangeStateUnclear ();
      }

      Assertion.IsTrue (ContainsObjectID (domainObject.ID));
      Assertion.IsTrue (_originalData.ContainsObjectID (domainObject.ID));
    }

    /// <summary>
    /// Unregisters the item with the given <paramref name="objectID"/> as an original item of this collection. This means the item is removed from 
    /// the <see cref="OriginalData"/> collection, and it is also removed from this <see cref="ChangeCachingCollectionDataDecorator"/> collection. If 
    /// the <see cref="OriginalData"/> collection does not contain the item, an exception is thrown. If this collection does not contain the item, it 
    /// is only removed from the <see cref="OriginalData"/>. This operation may invalidate the state cache.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to be unregistered.</param>
    public void UnregisterOriginalItem (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (!_originalData.ContainsObjectID (objectID))
      {
        var message = string.Format ("The original collection does not contain a domain object with ID '{0}'.", objectID);
        throw new InvalidOperationException (message);
      }

      if (WrappedData.ContainsObjectID (objectID))
      {
        // Standard case: Both collections contain the item; the item is removed from bothd

        // Remove the item from the WrappedData collection. That way, if the original collection has not yet been copied, it will automatically not 
        // contain the item and the state cache remains valid.
        WrappedData.Remove (objectID);

        // If the original collection has been copied, we must remove the item manually and invalidate the state cache: Collections previously 
        // different because the item was in different places might now be the same.
        if (_originalData.IsContentsCopied)
        {
          _originalData.Remove (objectID);
          OnChangeStateUnclear ();
        }
      }
      else
      {
        // Special case: The current collection does not contain the item

        // We must remove the item to the original collection only and raise a potential state change notification
        _originalData.Remove (objectID);
        OnChangeStateUnclear ();
      }
    }

    /// <summary>
    /// Sorts the data in this <see cref="ChangeCachingCollectionDataDecorator"/> and the data in the <see cref="OriginalData"/> collection
    /// using the given <paramref name="comparer"/>. This operation causes the change state to be invalidated if the original data is not the same
    /// as the current data.
    /// </summary>
    /// <param name="comparer">The comparer to use for sorting the data.</param>
    public void SortOriginalAndCurrent (IComparer<DomainObject> comparer)
    {
      ArgumentUtility.CheckNotNull ("comparer", comparer);

      Sort (WrappedData, comparer);

      // If the original collection has been copied, we must sort it manually. This might cause the change state cache to be wrong, so it is 
      // invalidated.
      if (_originalData.IsContentsCopied)
      {
        Sort (_originalData, comparer);
        OnChangeStateUnclear ();
      }
    }

    private static void Sort (IDomainObjectCollectionData collectionData, IComparer<DomainObject> comparer)
    {
      ArgumentUtility.CheckNotNull ("collectionData", collectionData);
      ArgumentUtility.CheckNotNull ("comparer", comparer);

      var items = collectionData.ToArray ();
      Array.Sort (items, comparer);
      collectionData.ReplaceContents (items);
    }

    protected override void OnDataChanged (OperationKind operation, DomainObject affectedObject, int index)
    {
      OnChangeStateUnclear();
      base.OnDataChanged (operation, affectedObject, index);
    }

    private void OnChangeStateUnclear ()
    {
      _isCacheUpToDate = false;
      RaiseStateUpdatedNotification (null);
    }

    private void SetCachedHasChangedFlag (bool hasChanged)
    {
      _cachedHasChangedFlag = hasChanged;
      _isCacheUpToDate = true;
      RaiseStateUpdatedNotification (hasChanged);
    }

    private void RaiseStateUpdatedNotification (bool? newChangedState)
    {
      _stateUpdateListener.StateUpdated (newChangedState);
    }
  }
}
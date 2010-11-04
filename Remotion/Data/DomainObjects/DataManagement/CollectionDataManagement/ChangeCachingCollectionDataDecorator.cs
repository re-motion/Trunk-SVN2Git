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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Decorates another <see cref="IDomainObjectCollectionData"/> object and provides an <see cref="HasChanged"/> method that determines
  /// whether the contents of that <see cref="IDomainObjectCollectionData"/> object has changed when compared to an original 
  /// <see cref="DomainObjectCollection"/> instance. The result of the comparison is cached until the <see cref="OnDataChanged"/> method is
  /// called or a modifying method is called on <see cref="ChangeCachingCollectionDataDecorator"/>.
  /// </summary>
  [Serializable]
  public class ChangeCachingCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    private readonly OriginalDomainObjectCollectionData _originalData;
    private readonly ICollectionDataStateUpdateListener _stateUpdateListener;

    private bool _isCacheUpToDate;
    private bool _cachedHasChangedFlag;

    public ChangeCachingCollectionDataDecorator (
        IDomainObjectCollectionData wrappedData, 
        ICollectionDataStateUpdateListener stateUpdateListener)
      : base (ArgumentUtility.CheckNotNull ("wrappedData", wrappedData))
    {
      ArgumentUtility.CheckNotNull ("stateUpdateListener", stateUpdateListener);

      _originalData = new OriginalDomainObjectCollectionData (this);
      _stateUpdateListener = stateUpdateListener;
    }

    public IDomainObjectCollectionData OriginalData
    {
      get { return _originalData; }
    }

    public bool IsCacheUpToDate
    {
      get { return _isCacheUpToDate; }
    }

    public bool HasChanged (ICollectionEndPointChangeDetectionStrategy strategy)
    {
      if (!_isCacheUpToDate)
      {
        var hasChanged = strategy.HasDataChanged (this, _originalData);
        SetCachedHasChangedFlag (hasChanged);
      }

      return _cachedHasChangedFlag;
    }

    public override IDomainObjectCollectionData GetDataStore ()
    {
      return this;
    }

    public override void Clear ()
    {
      _originalData.CopyOnWrite ();
      base.Clear ();
      OnDataChanged ();
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      _originalData.CopyOnWrite ();
      base.Insert (index, domainObject);
      OnDataChanged ();
    }

    public override bool Remove (DomainObject domainObject)
    {
      _originalData.CopyOnWrite ();
      var found = base.Remove (domainObject);
      if (found)
        OnDataChanged ();
      return found;
    }

    public override bool Remove (ObjectID objectID)
    {
      _originalData.CopyOnWrite ();
      var found = base.Remove (objectID);
      if (found)
        OnDataChanged ();
      return found;
    }

    public override void Replace (int index, DomainObject value)
    {
      _originalData.CopyOnWrite ();
      base.Replace (index, value);
      OnDataChanged ();
    }

    public void Commit ()
    {
      _originalData.RevertToActualData ();
      SetCachedHasChangedFlag (false);
    }

    private void OnDataChanged ()
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
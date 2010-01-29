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
  /// <see cref="DomainObjectCollection"/> instance. The result of the comparison is cached until the <see cref="InvalidateCache"/> method is
  /// called or a modifying method is called on <see cref="ChangeCachingCollectionDataDecorator"/>.
  /// </summary>
  [Serializable]
  public class ChangeCachingCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    private readonly DomainObjectCollection _originalData;
    private bool _isCacheUpToDate;
    private bool _cachedHasChangedFlag;

    public ChangeCachingCollectionDataDecorator (IDomainObjectCollectionData wrappedData, DomainObjectCollection originalData)
        : base (wrappedData)
    {
      ArgumentUtility.CheckNotNull ("originalData", originalData);

      _originalData = originalData;
    }

    public bool IsCacheUpToDate
    {
      get { return _isCacheUpToDate; }
    }

    public void InvalidateCache ()
    {
      _isCacheUpToDate = false;
    }

    public bool HasChanged (ICollectionEndPointChangeDetectionStrategy strategy)
    {
      if (!_isCacheUpToDate)
      {
        _cachedHasChangedFlag = strategy.HasDataChanged (WrappedData, _originalData);
        _isCacheUpToDate = true;
      }

      return _cachedHasChangedFlag;
    }

    public override void Clear ()
    {
      base.Clear ();
      InvalidateCache ();
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      base.Insert (index, domainObject);
      InvalidateCache ();
    }

    public override bool Remove (DomainObject domainObject)
    {
      var found = base.Remove (domainObject);
      if (found)
        InvalidateCache ();
      return found;
    }

    public override bool Remove (ObjectID objectID)
    {
      var found = base.Remove (objectID);
      if (found)
        InvalidateCache ();
      return found;
    }

    public override void Replace (int index, DomainObject value)
    {
      base.Replace (index, value);
      InvalidateCache ();
    }
  
  }
}
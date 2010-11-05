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
  /// Holds data originally stored in a <see cref="DomainObjectCollection"/>. Used by <see cref="ChangeCachingCollectionDataDecorator"/> to store
  /// the original values of the collection.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class by default delegates (read-only) to the actual value collection, until it is instructed to make a copy (<see cref="CopyOnWrite"/>). 
  /// After that, it will delegate to the copy, until it is instructed to go back to the actual value collection.
  /// </para>
  /// <para>
  /// Changes via <see cref="IDomainObjectCollectionData.GetDataStore"/> are forbidden because <see cref="ChangeCachingCollectionDataDecorator"/> must be able to rely on the
  /// fact that the original collection never changes.
  /// </para>
  /// </remarks>
  [Serializable]
  public class OriginalDomainObjectCollectionData : ReadOnlyCollectionDataDecorator
  {
    private readonly IDomainObjectCollectionData _actualData;

    public OriginalDomainObjectCollectionData (IDomainObjectCollectionData actualData)
        : base (ArgumentUtility.CheckNotNull ("actualData", actualData), false)
    {
      _actualData = actualData;
    }

    public void CopyOnWrite ()
    {
      if (WrappedData == _actualData)
        WrappedData = new DomainObjectCollectionData (WrappedData);
    }
    
    public void RevertToActualData ()
    {
      WrappedData = _actualData;
    }
  }
}
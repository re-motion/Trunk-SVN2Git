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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides extension methods for <see cref="DomainObjectCollection"/>.
  /// </summary>
  public static class DomainObjectCollectionExtensions
  {
    /// <summary>
    /// Adds all items of the given <see cref="DomainObjectCollection"/> to the <see cref="DomainObjectCollection"/>, that are not already part of it.
    /// This method is a convenienve method combining <see cref="DomainObjectCollection.Contains"/> and <see cref="DomainObjectCollection.AddRange"/>. If there are no changes made to this
    /// collection, the <see cref="DomainObjectCollection"/> method does not touch the associated end point (if any).
    /// </summary>
    /// <param name="collection">The collection to add items to.</param>
    /// <param name="sourceCollection">The collection to add items from. Must not be <see langword="null"/>.</param>
    /// <remarks>
    /// To check if an item is already part of the <see cref="DomainObject.ID"/> its <see cref="DomainObject"/> is used.
    /// <see cref="DomainObject.ID"/> does not check if the item references are identical. In case the two <see cref="DomainObject"/> contain
    /// different items with the same <see cref="DomainObjectCollection"/>, <see cref="System"/> will thus ignore those items.
    /// </remarks>
    /// <exception cref="Remotion.Data.DomainObjects.DataManagement"><paramref name="sourceCollection"/> is <see langword="null"/>.</exception>
    /// <exception cref="ClientTransaction">
    /// 	<paramref name="sourceCollection"/> contains a <see cref="DomainObjectCollection"/> that belongs to a <see cref="UnionWith"/> that is different from
    /// the <see cref="UnionWith"/> managing this collection.
    /// This applies only to <see cref="UnionWith"/>s that represent a relation.
    /// </exception>
    public static void UnionWith (this DomainObjectCollection collection, DomainObjectCollection sourceCollection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("sourceCollection", sourceCollection);
      
      collection.CheckNotReadOnly ("A read-only collection cannot be combined with another collection.");

      collection.AddRange (sourceCollection.Cast<DomainObject> ().Where (obj => !collection.Contains (obj.ID)));
    }

    /// <summary>
    /// Checks that the given <see cref="DomainObjectCollection"/> is not read only, throwing a <see cref="NotSupportedException"/> if it is.
    /// </summary>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The message the exception should have if one is thrown.</param>
    public static void CheckNotReadOnly (this DomainObjectCollection collection, string message)
    {
      if (collection.IsReadOnly)
        throw new NotSupportedException (message);
    }
  }
}
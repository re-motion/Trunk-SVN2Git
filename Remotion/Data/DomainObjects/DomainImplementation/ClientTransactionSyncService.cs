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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides APIs for checking whether the opposite relation properties in a bidirectional relation are out-of-sync, and - if yes -
  /// allows to synchronize them. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a bidirectional relation property is loaded from the underlying data source, re-store always tries to keep the two opposite
  /// sides in the relation in-sync. For example, in an Order-OrderItems relation, both the Order's relation property and 
  /// the individual OrderItems' relation properties should reflect the same relation data.
  /// </para>
  /// <para>
  /// There is one scenario, however, under which re-store cannot keep that promise of consistency. When a 1:n bidirectional relation
  /// is resolved from the underlying data source (eg., by accessing the <see cref="DomainObjectCollection"/> property), the 
  /// <see cref="ClientTransaction"/> loads the contents of the respective relation and stores it for further use. Now consider that 
  /// the data source is changed, and after that another item is loaded. If that item's relation property has a foreign key that would 
  /// qualify the item as part of the same <see cref="DomainObjectCollection"/> that was already resolved before, re-store cannot keep 
  /// up consistency between the foreign key property and the <see cref="DomainObjectCollection"/> without changing the existing collection's 
  /// contents.
  /// </para>
  /// <para>
  /// Here is an example illustrating this scenario:
  /// <code>
  /// var order = Order.GetObject (DomainObjectIDs.Order1);
  /// var orderItemsArray = order.OrderItems.ToArray(); // cause the full relation contents to be loaded and stored in-memory
  ///
  /// // data source now changes: an additional OrderItem with ID NewOrderItem is added, which points back to DomainObjectIDs.Order1
  /// 
  /// var newOrderItem = OrderItem.GetObject (DomainObjectIDs.NewOrderItem);
  /// 
  /// // prints "True" - the foreign key property points to DomainObjectIDs.Order1
  /// Console.WriteLine (newOrderItem.Order == order);
  ///
  /// // prints "False" - the relation is out-of-sync
  /// Console.WriteLine (order.OrderItems.ContainsObject (newOrderItem));
  /// </code>
  /// </para>
  /// <para>
  /// The <see cref="ClientTransactionSyncService"/> class allows users to check whether a relation is out-of-sync (<see cref="IsSynchronized"/>)
  /// and, if so, get re-store to synchronize the opposite sides in the relation (<see cref="Synchronize"/>):
  /// <code>
  /// var endPointID = RelationEndPointID.Create (newOrderItem, oi => oi.Order);
  /// 
  /// // Prints "False" - the relation is out-of-sync
  /// Console.WriteLine (ClientTransactionSyncService.IsSynchronized (ClientTransaction.Current, endPointID));
  /// 
  /// ClientTransactionSyncService.Synchronize (ClientTransaction.Current, endPointID);
  /// 
  /// // Prints "True" - the relation is now synchronized
  /// Console.WriteLine (ClientTransactionSyncService.IsSynchronized (ClientTransaction.Current, endPointID));
  /// 
  /// // prints "True" - the relation is now synchronized
  /// Console.WriteLine (order.OrderItems.ContainsObject (newOrderItem));
  /// </code>
  /// </para>
  /// </remarks>
  public static class ClientTransactionSyncService
  {
    /// <summary>
    /// Determines whether the given relation property is in-sync with the opposite relation property/properties.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to check the relation property in.</param>
    /// <param name="endPointID">The ID of the relation property to check. This contains the ID of the originating object and the
    /// relation property to check.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified relation property is synchronized; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   <paramref name="endPointID"/> denotes a unidirectional (or anonymous) relation property.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The relation property denoted by <paramref name="endPointID"/> has not yet been loaded into the given <paramref name="clientTransaction"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    ///   In the current implementation, 1:1 relations are always synchronized.
    /// </para>
    /// </remarks>
    public static bool IsSynchronized (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.RelationDefinition.RelationKind == RelationKindType.Unidirectional)
        throw new ArgumentException ("IsSynchronized cannot be called for unidirectional relation end-points.", "endPointID");

      var endPoint = clientTransaction.DataManager.RelationEndPointMap[endPointID];
      if (endPoint == null)
      {
        var message = string.Format (
            "The relation property '{0}' of object '{1}' has not yet been loaded into the given ClientTransaction.",
            endPointID.Definition.PropertyName,
            endPointID.ObjectID);
        throw new InvalidOperationException (message);
      }
      return endPoint.IsSynchronized;
    }

    /// <summary>
    /// Synchronizes the given relation property with its opposite relation property/properties.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to synchronize the relation property in.</param>
    /// <param name="endPointID">The ID of the relation property to synchronize. This contains the ID of the originating object and the
    /// relation property to check.</param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="endPointID"/> denotes a unidirectional (or anonymous) relation property.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The relation property denoted by <paramref name="endPointID"/> has not yet been loaded into the given <paramref name="clientTransaction"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    ///   If <paramref name="endPointID"/> denotes an object-valued end-point (eg., OrderItem.Order), only this one end-point is synchronized with
    ///   the opposite end-point.
    ///   If <paramref name="endPointID"/> denotes a collection-valued end-point (eg., Order.OrderItems), the end-point is synchronized with all 
    ///   opposite end-points.
    /// </para>
    /// <para>
    ///   If the relation is already synchronized, this method does nothing.
    /// </para>
    /// <para>
    ///   In the current implementation, 1:1 relations are always synchronized.
    /// </para>
    /// </remarks>
    public static void Synchronize (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      throw new NotImplementedException ();
    }
  }
}
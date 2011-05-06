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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality for unloading the data that a <see cref="ClientTransaction"/> stores for <see cref="DomainObject"/> instances and for
  /// relations. Use the methods of this class to remove unneeded data from memory and, more importantly, to reload data from the underlying 
  /// data source.
  /// </summary>
  public static class UnloadService
  {
    /// <summary>
    /// Unloads the virtual relation end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>. If the end point has not been loaded or has already been unloaded, this method does nothing.
    /// The relation must be unchanged in order to be unloaded.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The ID of the relation property to unload. This must denote a virtual relation end-point, ie., the relation side not 
    /// holding the foreign key property.</param>
    /// <exception cref="InvalidOperationException">The given end point is not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a virtual relation end point.</exception>
    /// <remarks>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static void UnloadVirtualEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckVirtualEndPointID (endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadVirtualEndPointsCommand (endPointID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      executor.ExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    /// <summary>
    /// Tries to unload the virtual end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>, returning a value indicating whether the unload operation succeeded. If the end point has not been loaded or
    /// has already been unloaded, this method does nothing.
    /// The relation must be unchanged in order to be unloaded.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The ID of the relation property to unload. This must denote a virtual relation end-point, ie., the relation side not 
    /// holding the foreign key property.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a virtual relation end point.</exception>
    /// <remarks>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static bool TryUnloadVirtualEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckVirtualEndPointID (endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadVirtualEndPointsCommand (endPointID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified 
    /// <paramref name="objectID"/>. The <see cref="DomainObject"/> reference 
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if 
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="objectID">The object ID.</param>
    /// <exception cref="InvalidOperationException">The object to be unloaded is not in unchanged state - or - the operation would affect an 
    /// opposite relation end-point that is not in unchanged state.</exception>
    /// <remarks>
    /// <para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object and their respective opposite virtual object 
    /// end-points. This means that unloading an object will unload a relation if and only if the object's <see cref="DataContainer"/> is holding 
    /// the foreign key for the relation.
    /// </para>
    /// <para>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </para>
    /// </remarks>
    public static void UnloadData (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadCommand (objectID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      executor.ExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified
    /// <paramref name="objectID"/>, returning a value indicating whether the unload operation succeeded. The <see cref="DomainObject"/> reference
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="objectID">The object ID.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <remarks>
    /// <para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object and their respective opposite virtual object 
    /// end-points. This means that unloading an object will unload a relation if and only if the object's <see cref="DataContainer"/> is holding 
    /// the foreign key for the relation.
    /// </para>
    /// 	<para>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </para>
    /// </remarks>
    public static bool TryUnloadData (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => tx.DataManager.CreateUnloadCommand (objectID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    /// <summary>
    /// Unloads the collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified 
    /// <see cref="ClientTransaction"/> as well as the data items stored by it. If the end point has not been loaded or has already been unloaded, 
    /// this method does nothing.
    /// The relation must be unchanged in order to be unloaded.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <exception cref="InvalidOperationException">The involved end points or one of the items it stores are not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static void UnloadCollectionEndPointAndData (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckCollectionEndPointID (endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => CreateUnloadCollectionEndPointAndDataCommand(tx, endPointID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      executor.ExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    /// <summary>
    /// Unloads the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/> as well as the data items stored by it, returning a value indicating whether the unload operation succeeded. 
    /// If the end point has not been loaded or has already been unloaded, this method returns <see langword="true" /> and does nothing.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from. The unload operation always affects the whole transaction 
    /// hierarchy.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in one transaction).</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// The unload operation is not atomic over the transaction hierarchy. It will start at the <see cref="ClientTransaction.LeafTransaction"/> 
    /// and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the transactions, 
    /// it will stop and throw an exception. At this point of time, the operation will have unloaded items from all the transactions where it 
    /// succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static bool TryUnloadCollectionEndPointAndData (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckCollectionEndPointID (endPointID);

      Func<ClientTransaction, IDataManagementCommand> commandFactory = tx => CreateUnloadCollectionEndPointAndDataCommand (tx, endPointID);
      var executor = new TransactionHierarchyCommandExecutor (commandFactory);
      return executor.TryExecuteCommandForTransactionHierarchy (clientTransaction);
    }

    private static void CheckCollectionEndPointID (RelationEndPointID endPointID)
    {
      if (endPointID.Definition.Cardinality != CardinalityType.Many || endPointID.Definition.IsAnonymous)
      {
        var message = string.Format ("The given end point ID '{0}' does not denote a collection-valued end-point.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }
    }

    private static void CheckVirtualEndPointID (RelationEndPointID endPointID)
    {
      if (!endPointID.Definition.IsVirtual)
      {
        var message = string.Format ("The given end point ID '{0}' does not denote a virtual end-point.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }
    }

    private static IDataManagementCommand CreateUnloadCollectionEndPointAndDataCommand (ClientTransaction tx, RelationEndPointID endPointID)
    {
      CheckCollectionEndPointID (endPointID);
      var endPoint = (ICollectionEndPoint) tx.DataManager.GetRelationEndPointWithoutLoading (endPointID);

      if (endPoint == null || !endPoint.IsDataComplete)
        return new NopCommand ();

      var unloadedObjectIDs = endPoint.Collection.Cast<DomainObject> ().Select (obj => obj.ID).ToArray ();
      var unloadDataCommand = tx.DataManager.CreateUnloadCommand (unloadedObjectIDs);

      var unloadEndPointCommand = tx.DataManager.CreateUnloadVirtualEndPointsCommand (endPointID);
      return new CompositeCommand (unloadDataCommand, unloadEndPointCommand);
    }
  }
}
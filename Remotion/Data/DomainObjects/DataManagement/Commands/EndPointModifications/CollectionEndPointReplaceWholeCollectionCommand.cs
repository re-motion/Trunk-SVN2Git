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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of the whole <see cref="CollectionEndPoint.Collection"/> collection, including the transformation
  /// of the involved <see cref="DomainObjectCollection"/> instances into stand-alone resp. associated collections.
  /// </summary>
  public class CollectionEndPointReplaceWholeCollectionCommand : RelationEndPointModificationCommand
  {
    private readonly ICollectionEndPoint _modifiedEndPoint;
    private readonly DomainObjectCollection _newOppositeCollection;
    private readonly Action<DomainObjectCollection> _collectionSetter;

    private readonly IAssociatableDomainObjectCollection _oldOppositeCollectionTransformer;
    private readonly IAssociatableDomainObjectCollection _newOppositeCollectionTransformer;

    private DomainObject[] _removedObjects;
    private DomainObject[] _addedObjects;

    public CollectionEndPointReplaceWholeCollectionCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObjectCollection newOppositeCollection,
        Action<DomainObjectCollection> collectionSetter,
        IAssociatableDomainObjectCollection oldOppositeCollectionTransformer,
        IAssociatableDomainObjectCollection newOppositeCollectionTransformer)
      : base (ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint), null, null)
    {
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);
      ArgumentUtility.CheckNotNull ("collectionSetter", collectionSetter);
      ArgumentUtility.CheckNotNull ("oldOppositeCollectionTransformer", oldOppositeCollectionTransformer);
      ArgumentUtility.CheckNotNull ("newOppositeCollectionTransformer", newOppositeCollectionTransformer);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _modifiedEndPoint = modifiedEndPoint;
      _newOppositeCollection = newOppositeCollection;
      _collectionSetter = collectionSetter;

      _oldOppositeCollectionTransformer = oldOppositeCollectionTransformer;
      _newOppositeCollectionTransformer = newOppositeCollectionTransformer;
    }

    public new ICollectionEndPoint ModifiedEndPoint
    {
      get { return _modifiedEndPoint; }
    }

    public DomainObjectCollection NewOppositeCollection
    {
      get { return _newOppositeCollection; }
    }

    public IAssociatableDomainObjectCollection OldOppositeCollectionTransformer
    {
      get { return _oldOppositeCollectionTransformer; }
    }

    public IAssociatableDomainObjectCollection NewOppositeCollectionTransformer
    {
      get { return _newOppositeCollectionTransformer; }
    }

    public DomainObject[] RemovedObjects
    {
      get
      {
        if (_removedObjects == null)
        {
          var oldOppositeObjects = ModifiedEndPoint.Collection.Cast<DomainObject> ();
          _removedObjects = oldOppositeObjects.Where (oldObject => !NewOppositeCollection.ContainsObject (oldObject)).ToArray();
        }
        return _removedObjects;
      }
    }

    public DomainObject[] AddedObjects
    {
      get
      {
        if (_addedObjects == null)
        {
          var newOppositeObjects = NewOppositeCollection.Cast<DomainObject> ();
          _addedObjects = newOppositeObjects.Where (newObject => !ModifiedEndPoint.Collection.ContainsObject (newObject)).ToArray();
        }
        return _addedObjects;
      }
    }

    protected override void ScopedNotifyClientTransactionOfBegin ()
    {
      for (int i = 0; i < RemovedObjects.Length; i++)
        RaiseClientTransactionBeginNotification (RemovedObjects[i], null);
      for (int i = 0; i < AddedObjects.Length; i++)
        RaiseClientTransactionBeginNotification (null, AddedObjects[i]);
    }

    protected override void ScopedBegin ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject ();

      for (int i = 0; i < RemovedObjects.Length; i++)
        domainObject.OnRelationChanging (new RelationChangingEventArgs (_modifiedEndPoint.Definition, RemovedObjects[i], null));
      for (int i = 0; i < AddedObjects.Length; i++)
        domainObject.OnRelationChanging (new RelationChangingEventArgs (_modifiedEndPoint.Definition, null, AddedObjects[i]));
    }

    public override void Perform ()
    {
      // only transform the old collection to stand-alone if it is still associated with this end point
      // rationale: during rollback, the old relation might have already been associated with another end-point, we must not overwrite this!
      if (OldOppositeCollectionTransformer.IsAssociatedWith (ModifiedEndPoint))
        OldOppositeCollectionTransformer.TransformToStandAlone();

      // we must always associate the new collection with the end point, however - even during rollback phase
      NewOppositeCollectionTransformer.TransformToAssociated (ModifiedEndPoint);

      // now make end point refer to the new collection by reference, too
      _collectionSetter (NewOppositeCollection); // this also touches the end point
      Assertion.IsTrue (ModifiedEndPoint.HasBeenTouched);
    }

    protected override void ScopedEnd ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject ();

      for (int i = AddedObjects.Length - 1; i >= 0; i--)
        domainObject.OnRelationChanged (new RelationChangedEventArgs (_modifiedEndPoint.Definition));
      for (int i = RemovedObjects.Length - 1; i >= 0; i--)
        domainObject.OnRelationChanged (new RelationChangedEventArgs (_modifiedEndPoint.Definition));
    }

    protected override void ScopedNotifyClientTransactionOfEnd ()
    {
      for (int i = RemovedObjects.Length - 1; i >= 0; i--)
        RaiseClientTransactionEndNotification (RemovedObjects[i], null);
      for (int i = AddedObjects.Length - 1; i >= 0; i--)
        RaiseClientTransactionEndNotification (null, AddedObjects[i]);
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional collection replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A replace operation of the form "customer.Orders = newOrders" involves the following steps:
    /// <list type="bullet">
    ///   <item>for each oldOrder the old collection (Orders) that's not in the new one: oldOrder.Customer = <see langword="null" />,</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer.Orders.Remove (newOrder),</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer = customer,</item>
    ///   <item>customer.Orders = newOrders.</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var domainObjectOfCollectionEndPoint = base.ModifiedEndPoint.GetDomainObject ();

      var commandsForRemoved = from oldObject in RemovedObjects
                               let endPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> (oldObject)
                               select endPoint.CreateRemoveCommand (domainObjectOfCollectionEndPoint); // oldOrder.Customer = null
      
      var commandsForAdded = from newObject in AddedObjects
                             let endPointOfNewObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> (newObject) // newOrder.Customer
                             let oldRelatedOfNewObject = endPointOfNewObject.GetOppositeObject (false) // newOrder.Customer
                             let endPointOfOldRelatedOfNewObject = endPointOfNewObject.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (oldRelatedOfNewObject) // newOrder.Customer.Orders
                             let removeCommand = endPointOfOldRelatedOfNewObject.CreateRemoveCommand (newObject) // newOrder.Customer.Orders.Remove (newOrder)
                             let setCommand = endPointOfNewObject.CreateSetCommand (domainObjectOfCollectionEndPoint) // newOrder.Customer = customer
                             from command in new[] { removeCommand, setCommand }
                             select command;

      return new ExpandedCommand (commandsForRemoved).CombineWith (commandsForAdded).CombineWith (this);
    }
  }
}
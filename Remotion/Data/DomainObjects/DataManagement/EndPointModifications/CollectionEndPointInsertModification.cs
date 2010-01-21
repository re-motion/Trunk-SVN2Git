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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents the insertion of an element into a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointInsertModification : RelationEndPointModification
  {
    private readonly int _index;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointInsertModification (
        ICollectionEndPoint modifiedEndPoint, 
        int index, 
        DomainObject insertedObject, 
        IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            ArgumentUtility.CheckNotNull ("insertedObject", insertedObject))
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModification is needed.", "modifiedEndPoint");

      _index = index;
      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.OppositeDomainObjects;
    }

    public int Index
    {
      get { return _index; }
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public override void Begin ()
    {
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginAdd (Index, NewRelatedObject);
      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Insert (Index, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndAdd (Index, NewRelatedObject);
      base.End();
    }

    /// <summary>
    /// Creates all modifications needed to perform a bidirectional insert operation into this collection end point.
    /// </summary>
    /// <remarks>
    /// An insert operation of the form "customer.Orders.Insert (insertedOrder, index)" needs three steps:
    /// <list type="bullet">
    ///   <item>insertedOrder.Customer = customer,</item>
    ///   <item>customer.Orders.Insert (insertedOrder, index), and</item>
    ///   <item>oldCustomer.Orders.Remove (insertedOrder) - with oldCustomer being the old customer of the inserted order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override IDataManagementCommand ExtendToAllRelatedObjects ()
    {
      // the end point that will be linked to the collection end point after the operation
      var insertedObjectEndPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> (NewRelatedObject);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfInsertedObject = insertedObjectEndPoint.GetOppositeObject (false);
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfInsertedObject = insertedObjectEndPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (oldRelatedObjectOfInsertedObject);

      return new CompositeDataManagementCommand (
          // insertedOrder.Customer = customer (previously oldCustomer)
          insertedObjectEndPoint.CreateSetModification (ModifiedEndPoint.GetDomainObject()),
          // customer.Orders.Insert (insertedOrder, index)
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfInsertedObject.CreateRemoveModification (NewRelatedObject));
    }
  }
}

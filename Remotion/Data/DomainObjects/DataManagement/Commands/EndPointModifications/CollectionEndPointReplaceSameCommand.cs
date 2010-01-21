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

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of an element in a <see cref="CollectionEndPoint"/> with itself. Calling <see cref="ExtendToAllRelatedObjects"/>
  /// results in an <see cref="IDataManagementCommand"/> that does not raise any events.
  /// </summary>
  public class CollectionEndPointReplaceSameCommand : RelationEndPointModificationCommand
  {
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointReplaceSameCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObject selfReplacedObject, 
        IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("selfReplacedObject", selfReplacedObject),
            ArgumentUtility.CheckNotNull ("selfReplacedObject", selfReplacedObject))
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.OppositeDomainObjects;
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
      // do not issue any change notifications, a self-replacement is not a change
    }

    public override void Perform ()
    {
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      // do not issue any change notifications, a self-replacement is not a change
    }

    public override void NotifyClientTransactionOfBegin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void NotifyClientTransactionOfEnd ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    /// <summary>
    /// Creates all commands needed to perform a self-replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A self-replace operation of the form "customer.Orders[index] = customer.Orders[index]" needs two steps:
    /// <list type="bullet">
    ///   <item>customer.Orders.Touch() and</item>
    ///   <item>customer.Orders[index].Touch().</item>
    /// </list>
    /// No change notifications are sent for this operation.
    /// </remarks>
    public override IDataManagementCommand ExtendToAllRelatedObjects ()
    {
      var endPointOfRelatedObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> (OldRelatedObject);

      return new CompositeDataManagementCommand (
          this,
          new RelationEndPointTouchCommand (endPointOfRelatedObject));
    }
  }
}

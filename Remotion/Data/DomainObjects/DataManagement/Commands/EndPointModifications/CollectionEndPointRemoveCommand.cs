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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the removal of an element from a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointRemoveCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;

    private readonly ICollectionEndPointDataKeeper _dataKeeper;

    private readonly DomainObjectCollection _modifiedCollection;
    private readonly IObjectEndPoint _removedEndPoint;

    public CollectionEndPointRemoveCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObject removedObject, 
        ICollectionEndPointDataKeeper dataKeeper,
        IRelationEndPointProvider endPointProvider)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("removedObject", removedObject),
            null)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _index = modifiedEndPoint.Collection.IndexOf (OldRelatedObject);

      _dataKeeper = dataKeeper;
      _modifiedCollection = modifiedEndPoint.Collection;

      _removedEndPoint = (IObjectEndPoint) GetOppositeEndPoint (modifiedEndPoint, removedObject, endPointProvider);
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IObjectEndPoint RemovedEndPoint
    {
      get { return _removedEndPoint; }
    }

    protected override void ScopedBegin ()
    {
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginRemove (_index, OldRelatedObject);
      base.ScopedBegin ();
    }

    public override void Perform ()
    {
      DataKeeper.Remove (_removedEndPoint);
      ModifiedEndPoint.Touch ();
    }

    protected override void ScopedEnd ()
    {
      base.ScopedEnd ();
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndRemove (_index, OldRelatedObject);
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional remove operation from this collection end point.
    /// </summary>
    /// <remarks>
    /// A remove operation of the form "customer.Orders.Remove (RemovedOrder)" needs two steps:
    /// <list type="bullet">
    ///   <item>RemovedOrder.Customer = null and</item>
    ///   <item>customer.Orders.Remove (removedOrder).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (
          _removedEndPoint.CreateRemoveCommand (ModifiedEndPoint.GetDomainObject ()), 
          this);
    }
  }
}

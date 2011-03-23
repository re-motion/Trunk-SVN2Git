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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the lazy-loading state of a <see cref="CollectionEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  public interface ICollectionEndPointLoadState : IFlattenedSerializable
  {
    bool IsDataComplete ();
    void EnsureDataComplete (ICollectionEndPoint collectionEndPoint);

    void MarkDataComplete (ICollectionEndPoint collectionEndPoint, DomainObject[] items, Action<ICollectionEndPointDataKeeper> stateSetter);
    void MarkDataIncomplete (ICollectionEndPoint collectionEndPoint, Action<ICollectionEndPointDataKeeper> stateSetter);

    ReadOnlyCollectionDataDecorator GetCollectionData (ICollectionEndPoint collectionEndPoint);
    // TODO 3816: Refactor to match GetCollectionData
    DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint collectionEndPoint);

    void RegisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint);

    void RegisterCurrentOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint);

    bool IsSynchronized (ICollectionEndPoint collectionEndPoint);
    void Synchronize (ICollectionEndPoint collectionEndPoint);

    void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    IDataManagementCommand CreateSetCollectionCommand (ICollectionEndPoint collectionEndPoint, DomainObjectCollection newCollection, Action<DomainObjectCollection> collectionSetter);
    IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject);
    IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint);
    IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index);
    IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject);
    IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject);

    void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint);
    // TODO 3816: Move implementation to CollectionEndPoint (from CompleteCollectionEndPointLoadState, use GetCollectionData instead of DataKeeper)
    void CheckMandatory (ICollectionEndPoint collectionEndPoint);

    bool HasChanged ();

    void Commit ();
    void Rollback ();
  }
}
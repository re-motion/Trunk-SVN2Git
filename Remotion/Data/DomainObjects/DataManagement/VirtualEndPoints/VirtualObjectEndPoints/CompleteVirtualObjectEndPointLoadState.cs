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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualObjectEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteVirtualObjectEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, DomainObject, IVirtualObjectEndPointDataKeeper>, IVirtualObjectEndPointLoadState
  {
    public CompleteVirtualObjectEndPointLoadState (
        IVirtualObjectEndPointDataKeeper dataKeeper, 
        IRelationEndPointProvider endPointProvider, 
        ClientTransaction clientTransaction)
        : base (dataKeeper, endPointProvider, clientTransaction)
    {
    }

    public override DomainObject GetData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return DataKeeper.CurrentOppositeObject;
    }

    public override DomainObject GetOriginalData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return DataKeeper.OriginalOppositeObject;
    }

    public override void SetDataFromSubTransaction (IVirtualObjectEndPoint endPoint, IVirtualEndPointLoadState<IVirtualObjectEndPoint, DomainObject, IVirtualObjectEndPointDataKeeper> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      var sourceCompleteLoadState = ArgumentUtility.CheckNotNullAndType<CompleteVirtualObjectEndPointLoadState> ("sourceLoadState", sourceLoadState);

      DataKeeper.SetDataFromSubTransaction (sourceCompleteLoadState.DataKeeper, EndPointProvider);
    }

    public void MarkDataComplete (IVirtualObjectEndPoint endPoint, DomainObject item, Action<IVirtualObjectEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      MarkDataComplete (endPoint, new[] { item }, stateSetter);
    }

    public override void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (DataKeeper.OriginalOppositeEndPoint != null)
      {
        var message = string.Format (
            "The object end-point '{0}' cannot be synchronized with the virtual object end-point '{1}' because the virtual relation property already "
            + "refers to another object ('{2}'). To synchronize '{0}', use UnloadService to unload either object '{2}' or the virtual object "
            + "end-point '{1}'.",
            oppositeEndPoint.ID,
            DataKeeper.EndPointID,
            DataKeeper.OriginalOppositeEndPoint.ObjectID);
        throw new InvalidOperationException (message);
      }

      base.SynchronizeOppositeEndPoint (oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetCommand (IVirtualObjectEndPoint virtualObjectEndPoint, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);

      var oldRelatedObject = DataKeeper.CurrentOppositeObject;
      if (DataKeeper.OriginalItemWithoutEndPoint != null)
      {
        var message = string.Format (
            "The virtual property '{0}' of object '{1}' cannot be set because the property is "
            + "out of sync with the opposite object property '{2}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{0}' property.",
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition ().PropertyName);
        throw new InvalidOperationException (message);
      }

      if (oldRelatedObject == newRelatedObject)
      {
        return new ObjectEndPointSetSameCommand (virtualObjectEndPoint, domainObject => DataKeeper.CurrentOppositeObject = domainObject);
      }
      else
      {
        if (newRelatedObject != null)
          CheckAddedObject (newRelatedObject);

        return new ObjectEndPointSetOneOneCommand (virtualObjectEndPoint, newRelatedObject, domainObject => DataKeeper.CurrentOppositeObject = domainObject);
      }
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualObjectEndPoint virtualObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
            + "virtual property '{1}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition ().PropertyName,
            UnsynchronizedOppositeEndPoints.First ().ObjectID);
        throw new InvalidOperationException (message);
      }

      if (DataKeeper.OriginalItemWithoutEndPoint != null)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because its virtual property '{1}' is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition ().PropertyName,
            DataKeeper.OriginalItemWithoutEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      return new ObjectEndPointDeleteCommand (virtualObjectEndPoint, domainObject => DataKeeper.CurrentOppositeObject = domainObject);
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataKeeper.OriginalOppositeEndPoint == null ? new IRealObjectEndPoint[0] : new[] { DataKeeper.OriginalOppositeEndPoint };
    }

    protected override IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ()
    {
      return DataKeeper.OriginalItemWithoutEndPoint == null ? new DomainObject[0] : new[] { DataKeeper.OriginalItemWithoutEndPoint };
    }

    protected override bool HasUnsynchronizedCurrentOppositeEndPoints ()
    {
      return DataKeeper.CurrentOppositeEndPoint != null && !DataKeeper.CurrentOppositeEndPoint.IsSynchronized;
    }

    private void CheckAddedObject (DomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be set into the virtual property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the virtual property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition ().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    #region Serialization

    public CompleteVirtualObjectEndPointLoadState (FlattenedDeserializationInfo info)
      : base (info)
    {
    }

    #endregion Serialization
  }
}
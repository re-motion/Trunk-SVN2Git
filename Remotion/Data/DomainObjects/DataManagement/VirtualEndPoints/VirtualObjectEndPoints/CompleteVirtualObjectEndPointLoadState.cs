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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualObjectEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteVirtualObjectEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, ObjectID, IVirtualObjectEndPointDataKeeper>, IVirtualObjectEndPointLoadState
  {
    public CompleteVirtualObjectEndPointLoadState (
        IVirtualObjectEndPointDataKeeper dataKeeper, 
        IRelationEndPointProvider endPointProvider, 
        ClientTransaction clientTransaction)
        : base (dataKeeper, endPointProvider, clientTransaction)
    {
    }

    public override ObjectID GetData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return DataKeeper.CurrentOppositeObjectID;
    }

    public override ObjectID GetOriginalData (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return DataKeeper.OriginalOppositeObjectID;
    }

    public override void SetValueFrom (IVirtualObjectEndPoint endPoint, IVirtualObjectEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      DataKeeper.CurrentOppositeObjectID = sourceEndPoint.OppositeObjectID;
    }

    public IDataManagementCommand CreateSetCommand (IVirtualObjectEndPoint virtualObjectEndPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (DataKeeper.CurrentOppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand (virtualObjectEndPoint, oppositeObjectIDSetter);
      else
        return new ObjectEndPointSetOneOneCommand (virtualObjectEndPoint, newRelatedObject, oppositeObjectIDSetter);
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualObjectEndPoint virtualObjectEndPoint, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      return new ObjectEndPointDeleteCommand (virtualObjectEndPoint, oppositeObjectIDSetter);
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

    #region Serialization

    public CompleteVirtualObjectEndPointLoadState (FlattenedDeserializationInfo info)
      : base (info)
    {
    }

    #endregion Serialization
  }
}
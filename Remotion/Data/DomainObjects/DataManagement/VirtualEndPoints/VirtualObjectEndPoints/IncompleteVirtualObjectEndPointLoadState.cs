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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualObjectEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteVirtualObjectEndPointLoadState 
      : IncompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, ObjectID, IVirtualObjectEndPointDataKeeper>, IVirtualObjectEndPointLoadState
  {
    public IncompleteVirtualObjectEndPointLoadState (IVirtualObjectEndPointDataKeeper dataKeeper, IRelationEndPointLazyLoader lazyLoader, IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> dataKeeperFactory)
        : base(dataKeeper, lazyLoader, dataKeeperFactory)
    {
    }

    public IDataManagementCommand CreateSetCommand (IVirtualObjectEndPoint virtualObjectEndPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      virtualObjectEndPoint.EnsureDataComplete();
      return virtualObjectEndPoint.CreateSetCommand (newRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualObjectEndPoint virtualObjectEndPoint, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      virtualObjectEndPoint.EnsureDataComplete();
      return virtualObjectEndPoint.CreateDeleteCommand();
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataKeeper.OriginalOppositeEndPoint == null ? new IRealObjectEndPoint[0] : new[] { DataKeeper.OriginalOppositeEndPoint };
    }

    #region Serialization

    public IncompleteVirtualObjectEndPointLoadState (FlattenedDeserializationInfo info)
        : base (info)
    {
    }

    #endregion Serialization
  }
}
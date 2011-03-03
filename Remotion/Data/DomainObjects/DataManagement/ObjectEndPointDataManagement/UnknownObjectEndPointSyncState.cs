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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement
{
  /// <summary>
  /// Represents the synchronization state of an <see cref="ObjectEndPoint"/> whose opposite end-point is not loaded/complete yet.
  /// In this case, the synchronization state is unknown until the opposite end-point is loaded. Any access to the sync state will cause the
  /// opposite end-point to be loaded.
  /// </summary>
  public class UnknownObjectEndPointSyncState : IObjectEndPointSyncState
  {
    private readonly IRelationEndPointLazyLoader _lazyLoader;

    public UnknownObjectEndPointSyncState (IRelationEndPointLazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      _lazyLoader = lazyLoader;
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public bool IsSynchronized (IObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _lazyLoader.LoadOppositeEndPoint (endPoint);

      return endPoint.IsSynchronized;
    }

    public void Synchronize (IObjectEndPoint endPoint, IRelationEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _lazyLoader.LoadOppositeEndPoint (endPoint);

      endPoint.Synchronize (oppositeEndPoint);
    }

    public IDataManagementCommand CreateDeleteCommand (IObjectEndPoint endPoint, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      _lazyLoader.LoadOppositeEndPoint (endPoint);

      return endPoint.CreateDeleteCommand();
    }

    public IDataManagementCommand CreateSetCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("newRelatedObject", newRelatedObject);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      _lazyLoader.LoadOppositeEndPoint (endPoint);

      return endPoint.CreateSetCommand (newRelatedObject);
    }

    #region Serialization

    public UnknownObjectEndPointSyncState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
    }

    #endregion
  }
}